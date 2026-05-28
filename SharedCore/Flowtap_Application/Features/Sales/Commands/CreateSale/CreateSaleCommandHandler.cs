using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Common.Notifications;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.CreateSale;

public class CreateSaleCommandHandler(IApplicationDbContext db, IDateTimeService dateTime, IPublisher publisher)
    : IRequestHandler<CreateSaleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSaleCommand request, CancellationToken ct)
    {
        // ── Idempotency ───────────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            var existing = await db.Sales
                .FirstOrDefaultAsync(s => s.IdempotencyKey == request.IdempotencyKey
                    && s.CompanyId == request.CompanyId, ct);
            if (existing is not null) return Result<Guid>.Success(existing.Id);
        }

        if (!Enum.TryParse<SaleSource>(request.Source, true, out var source))
            source = SaleSource.POS;

        // ── Walk-in client fallback ───────────────────────────────────────────
        var clientId = request.ClientId
            ?? await GetOrCreateWalkInClientAsync(request.CompanyId, request.LocationId, ct);

        // ── Build sale items ──────────────────────────────────────────────────
        var items = request.Items.Select(i =>
        {
            // Discount applied first, then tax added on top (exclusive pricing)
            var lineNet   = i.UnitPrice * i.Quantity - i.DiscountAmount;
            var lineTotal = lineNet * (1 + i.TaxPercent / 100);
            return new SaleItem
            {
                ProductId       = i.ProductId,
                ProductName     = i.ProductName,
                Type            = Enum.TryParse<SaleItemType>(i.Type, true, out var t) ? t : SaleItemType.Product,
                Quantity        = i.Quantity,
                UnitPrice       = i.UnitPrice,
                TaxPercent      = i.TaxPercent,
                DiscountPercent = i.DiscountPercent,
                DiscountAmount  = i.DiscountAmount,
                Total           = Math.Round(lineTotal, 2),
                SerialNumber    = i.SerialNumber,
            };
        }).ToList();

        // ── Compute totals ────────────────────────────────────────────────────
        var subTotal      = Math.Round(items.Sum(i => i.UnitPrice * i.Quantity), 2);
        var discountTotal = Math.Round(items.Sum(i => i.DiscountAmount), 2);
        var taxAmount     = Math.Round(items.Sum(i => (i.UnitPrice * i.Quantity - i.DiscountAmount) * i.TaxPercent / 100), 2);
        var total         = subTotal - discountTotal + taxAmount;

        // ── Create sale ───────────────────────────────────────────────────────
        var count = await db.Sales.CountAsync(s => s.CompanyId == request.CompanyId, ct);

        var sale = new Sale
        {
            CompanyId            = request.CompanyId,
            LocationId           = request.LocationId,
            ClientId             = clientId,
            TransactionNumber    = $"INV-{count + 1:D6}",
            Source               = source,
            TicketId             = request.TicketId,
            SubTotal             = subTotal,
            DiscountAmount       = discountTotal,
            TaxAmount            = taxAmount,
            TotalAmount          = Math.Round(total, 2),
            Status               = SaleStatus.Draft,
            Notes                = request.Notes,
            IdempotencyKey       = request.IdempotencyKey,
            CashierEmployeeId    = request.EmployeeId,
            Items                = items,
            History           =
            [
                new SaleHistory
                {
                    Message   = $"Sale {$"INV-{count + 1:D6}"} created via {source}. Total: {total:C}.",
                    CreatedAt = dateTime.UtcNow,
                }
            ]
        };

        // ── Load ticket advance (if this sale closes a service ticket) ──────────
        // The advance was already collected and stored on ticket.Financials.Prepayment.
        // We deduct it from the effective "paid so far" so the status calculation is correct,
        // and add a history entry so the receipt shows the full payment picture.
        // Ticket advance is supplied by the caller (Repair module) via TicketPrepayment
        // so we never need to query ServiceTickets from this project.
        // Ticket linking (ticket.SaleId) is done via SaleCreatedForTicketNotification.
        decimal ticketAdvance = request.TicketPrepayment;
        string? ticketNumber  = request.TicketNumber;

        // ── Resolve payment accounts BEFORE touching the sale context ────────
        // ResolvePaymentAccountAsync may call SaveChangesAsync internally (to
        // auto-create accounts / mappings). We must finish all of that work
        // while the Sale/Payment entities are NOT yet in the change tracker —
        // otherwise EF relationship-fixup drags the unsaved Sale into an
        // intermediate save, giving it a non-null RowVersion, which then causes
        // a DbUpdateConcurrencyException when we try to update Status later.
        var resolvedPayments = new List<(PaymentMethod Method, PaymentPurpose Purpose,
            decimal Amount, Guid AccountId, string? Reference)>();

        foreach (var p in request.Payments ?? [])
        {
            if (!Enum.TryParse<PaymentMethod>(p.Method, true, out var method))
                continue;
            if (!Enum.TryParse<PaymentPurpose>(p.Purpose, true, out var purpose))
                purpose = PaymentPurpose.Final;

            var accountId = await ResolvePaymentAccountAsync(
                request.CompanyId, request.LocationId, method, ct);

            resolvedPayments.Add((method, purpose, p.Amount, accountId, p.Reference));
        }

        // ── Compute final status (advance + inline payments) ──────────────────
        // ticketAdvance was already collected; resolvedPayments are collected now.
        var totalPaid = ticketAdvance + resolvedPayments.Sum(p => p.Amount);

        if (ticketAdvance > 0)
        {
            sale.History.Add(new SaleHistory
            {
                SaleId    = sale.Id,
                Message   = $"Advance of {ticketAdvance:C} applied from ticket {ticketNumber}.",
                CreatedAt = dateTime.UtcNow,
            });
        }

        if (totalPaid >= sale.TotalAmount)
        {
            sale.Status = SaleStatus.Completed;
            sale.History.Add(new SaleHistory
            {
                SaleId    = sale.Id,
                Message   = "Sale completed — fully paid.",
                CreatedAt = dateTime.UtcNow,
            });
        }
        else if (totalPaid > 0)
        {
            sale.History.Add(new SaleHistory
            {
                SaleId    = sale.Id,
                Message   = $"Partial payment received. Remaining: {sale.TotalAmount - totalPaid:C}.",
                CreatedAt = dateTime.UtcNow,
            });
        }

        // ── Add sale graph + payments in one batch → single SaveChangesAsync ──
        db.Sales.Add(sale);

        foreach (var (method, purpose, amount, accountId, reference) in resolvedPayments)
        {
            db.Payments.Add(new Payment
            {
                CompanyId         = request.CompanyId,
                SaleId            = sale.Id,
                Amount            = amount,
                Method            = method,
                Purpose           = purpose,
                AccountId         = accountId,
                ExternalReference = reference,
                PaidAt            = dateTime.UtcNow,
            });

            sale.History.Add(new SaleHistory
            {
                SaleId    = sale.Id,
                Message   = $"Payment of {amount:C} received via {method} ({purpose}).",
                CreatedAt = dateTime.UtcNow,
            });
        }

        await db.SaveChangesAsync(ct);

        // ── Notify Repair module to link ticket → sale ───────────────────────
        // SaleCreatedForTicketNotification is handled by Flowtap_Repair's
        // SaleCreatedForTicketHandler, which sets ticket.SaleId and IsPaid.
        if (request.TicketId.HasValue)
        {
            await publisher.Publish(new SaleCreatedForTicketNotification(
                SaleId:          sale.Id,
                TicketId:        request.TicketId.Value,
                CompanyId:       request.CompanyId,
                IsSaleCompleted: sale.Status == SaleStatus.Completed,
                TicketPrepayment: ticketAdvance), ct);
        }

        // ── Deduct stock ──────────────────────────────────────────────────────
        var warehouse = await db.Warehouses
            .FirstOrDefaultAsync(w => w.CompanyId == request.CompanyId, ct);

        if (warehouse is not null)
        {
            foreach (var item in sale.Items)
            {
                var stock = await db.WarehouseStocks
                    .FirstOrDefaultAsync(ws =>
                        ws.WarehouseId == warehouse.Id &&
                        ws.ProductId == item.ProductId &&
                        ws.CompanyId == request.CompanyId, ct);

                var before = stock?.Quantity ?? 0;
                var after  = before - item.Quantity;

                if (stock is not null)
                {
                    stock.Quantity        = Math.Max(0, after);
                    stock.LastMovementAt  = dateTime.UtcNow;
                    stock.LastSoldAt      = dateTime.UtcNow;
                }

                db.InventoryTransactions.Add(new InventoryTransaction
                {
                    CompanyId       = request.CompanyId,
                    ProductId       = item.ProductId,
                    WarehouseId     = warehouse.Id,
                    Type            = InventoryTransactionType.Sale,
                    Quantity        = item.Quantity,
                    QuantityBefore  = before,
                    QuantityAfter   = Math.Max(0, after),
                    CostPrice       = 0,
                    Reference       = sale.TransactionNumber!,
                    RelatedEntityId = sale.Id,
                    TransactionDate = dateTime.UtcNow,
                });
            }

            await db.SaveChangesAsync(ct);
        }

        return Result<Guid>.Success(sale.Id);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Resolves the PaymentAccount for a given method at a given location.
    /// Prefers PaymentMethodMapping (store-specific config); falls back to any matching
    /// account type; auto-creates account + mapping on first run.
    /// </summary>
    private async Task<Guid> ResolvePaymentAccountAsync(
        Guid companyId, Guid locationId, PaymentMethod method, CancellationToken ct)
    {
        // 1. Check PaymentMethodMapping for this location
        var mapping = await db.PaymentMethodMappings
            .FirstOrDefaultAsync(m =>
                m.CompanyId == companyId &&
                m.LocationId == locationId &&
                m.Method == method, ct);

        if (mapping is not null) return mapping.PaymentAccountId;

        // 2. Find any existing PaymentAccount of the matching type for this company
        var accountType = MethodToAccountType(method);
        var account = await db.PaymentAccounts
            .FirstOrDefaultAsync(a =>
                a.CompanyId == companyId &&
                a.Type == accountType &&
                a.IsActive, ct);

        // 3. Auto-create if none exists
        if (account is null)
        {
            account = new PaymentAccount
            {
                CompanyId = companyId,
                Name      = method.ToString(),
                Type      = accountType,
                IsActive  = true,
            };
            db.PaymentAccounts.Add(account);
            await db.SaveChangesAsync(ct);
        }

        // 4. Create the mapping so future lookups skip this logic
        db.PaymentMethodMappings.Add(new PaymentMethodMapping
        {
            CompanyId        = companyId,
            LocationId       = locationId,
            Method           = method,
            PaymentAccountId = account.Id,
        });
        await db.SaveChangesAsync(ct);

        return account.Id;
    }

    private static PaymentAccountType MethodToAccountType(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash       => PaymentAccountType.Cash,
        PaymentMethod.Card       => PaymentAccountType.Bank,
        PaymentMethod.NetBanking => PaymentAccountType.Bank,
        _                        => PaymentAccountType.Gateway,  // UPI, Wallet
    };

    private async Task<Guid> GetOrCreateWalkInClientAsync(Guid companyId, Guid locationId, CancellationToken ct)
    {
        var client = await db.Clients
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Name == "Walk-in Customer", ct);

        if (client is not null) return client.Id;

        client = new Client
        {
            CompanyId  = companyId,
            LocationId = locationId,
            Name       = "Walk-in Customer",
            Type       = ClientType.Individual,
        };
        db.Clients.Add(client);
        await db.SaveChangesAsync(ct);
        return client.Id;
    }
}
