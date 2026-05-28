using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Repair.Domain.Entities;
using Flowtap_Repair.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Commands.CollectTicket;

public class CollectTicketCommandHandler(IRepairDbContext db, IDateTimeService dateTime)
    : IRequestHandler<CollectTicketCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CollectTicketCommand request, CancellationToken ct)
    {
        // ── Step 1: resolve payment method (if final payment is being made) ─────────
        PaymentMethod finalMethod = PaymentMethod.Cash;
        if (request.FinalPaymentAmount > 0)
        {
            var methodStr = request.FinalPaymentMethod ?? "Cash";
            if (!Enum.TryParse<PaymentMethod>(methodStr, true, out finalMethod))
                return Result<Guid>.Failure($"Invalid payment method: {request.FinalPaymentMethod}");
        }

        // ── Step 2: lightweight projection — LocationId without tracking ─────────────
        // Account resolution below may SaveChangesAsync; ticket must not be tracked yet.
        var ticketInfo = await db.ServiceTickets
            .Where(t => t.Id == request.TicketId && t.CompanyId == request.CompanyId)
            .Select(t => new { t.LocationId, t.Status, t.SaleId, t.TicketNumber })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(nameof(ServiceTicket), request.TicketId);

        if (ticketInfo.Status == TicketStatus.Canceled)
            return Result<Guid>.Failure("Cannot collect a cancelled ticket.");

        // If sale already exists (e.g. admin marked Done directly), just return existing SaleId
        if (ticketInfo.SaleId.HasValue)
            return Result<Guid>.Success(ticketInfo.SaleId.Value);

        // ── Step 3: resolve payment account for final payment ────────────────────────
        var finalAccountId = Guid.Empty;
        if (request.FinalPaymentAmount > 0)
        {
            finalAccountId = request.AccountId ?? Guid.Empty;
            if (finalAccountId == Guid.Empty)
                finalAccountId = await ResolvePaymentAccountAsync(
                    request.CompanyId, ticketInfo.LocationId, finalMethod, ct);
        }

        // ── Step 4: load ticket with Items (tracking begins here) ────────────────────
        var ticket = await db.ServiceTickets
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException(nameof(ServiceTicket), request.TicketId);

        // ── Step 5: load existing advance payments (before Sale enters change tracker) ─
        var advancePayments = await db.Payments
            .Where(p => p.TicketId == ticket.Id && p.CompanyId == request.CompanyId && p.SaleId == null)
            .ToListAsync(ct);

        var totalAdvance = advancePayments.Sum(p => p.Amount);
        var finalAmount  = request.FinalPaymentAmount > 0 ? request.FinalPaymentAmount.Value : 0m;
        var totalPaid    = totalAdvance + finalAmount;

        // ── Step 6: map ticket items → SaleItems ────────────────────────────────────
        var saleItems = ticket.Items.Select(i =>
        {
            var saleItemType = i.Type switch
            {
                TicketItemType.Service => SaleItemType.Service,
                TicketItemType.Part    => SaleItemType.Part,
                _                      => SaleItemType.Product,
            };

            var lineNet   = i.Price * i.Quantity - i.DiscountAmount;
            var lineTotal = lineNet * (1 + i.TaxPercent / 100);

            return new SaleItem
            {
                ProductId       = i.ItemReferenceId,
                ProductName     = i.Name,
                Type            = saleItemType,
                Quantity        = i.Quantity,
                UnitPrice       = i.Price,
                TaxPercent      = i.TaxPercent,
                DiscountPercent = 0,
                DiscountAmount  = i.DiscountAmount,
                Total           = Math.Round(lineTotal, 2),
            };
        }).ToList();

        // ── Step 7: compute sale totals ──────────────────────────────────────────────
        var subTotal      = Math.Round(saleItems.Sum(i => i.UnitPrice * i.Quantity), 2);
        var discountTotal = Math.Round(saleItems.Sum(i => i.DiscountAmount), 2);
        var taxAmount     = Math.Round(saleItems.Sum(i => (i.UnitPrice * i.Quantity - i.DiscountAmount) * i.TaxPercent / 100), 2);
        var saleTotal     = Math.Round(subTotal - discountTotal + taxAmount, 2);

        // Fall back to Financials.TotalCost when items carry no prices
        if (saleTotal == 0 && (ticket.Financials?.TotalCost ?? 0) > 0)
            saleTotal = ticket.Financials!.TotalCost;

        // ── Step 8: build sale history ───────────────────────────────────────────────
        var count    = await db.Sales.CountAsync(s => s.CompanyId == request.CompanyId, ct);
        var txNumber = $"INV-{count + 1:D6}";
        var status   = totalPaid >= saleTotal ? SaleStatus.Completed : SaleStatus.Draft;

        var history = new List<SaleHistory>
        {
            new() { Message = $"Sale {txNumber} created on client collection of ticket {ticket.TicketNumber}.", CreatedAt = dateTime.UtcNow }
        };

        if (totalAdvance > 0)
            history.Add(new SaleHistory { Message = $"Advance of {totalAdvance:C} applied from prior ticket payments.", CreatedAt = dateTime.UtcNow });

        if (finalAmount > 0)
            history.Add(new SaleHistory { Message = $"Final payment of {finalAmount:C} received via {finalMethod}.", CreatedAt = dateTime.UtcNow });

        if (status == SaleStatus.Completed)
            history.Add(new SaleHistory { Message = "Sale completed — fully paid at collection.", CreatedAt = dateTime.UtcNow });
        else if (totalPaid > 0)
            history.Add(new SaleHistory { Message = $"Remaining balance: {saleTotal - totalPaid:C}.", CreatedAt = dateTime.UtcNow });

        // ── Step 9: resolve ClientId — guard against stale/placeholder GUIDs ───────
        // If the stored ClientId no longer references a real Client row (e.g. the ticket
        // was created with a Swagger placeholder before the client module was set up),
        // fall back to the company's Walk-in Customer so the FK constraint is never violated.
        var resolvedClientId = ticket.ClientId;
        var clientExists = await db.Clients
            .AnyAsync(c => c.Id == resolvedClientId && c.CompanyId == request.CompanyId, ct);

        if (!clientExists)
        {
            var walkIn = await db.Clients
                .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId && c.Name == "Walk-in Customer", ct);

            if (walkIn is null)
            {
                walkIn = new Flowtap_Domain.BoundedContexts.Modules.Sales.Entities.Client
                {
                    CompanyId  = request.CompanyId,
                    LocationId = ticket.LocationId,
                    Name       = "Walk-in Customer",
                    Type       = Flowtap_Domain.BoundedContexts.Modules.Sales.Enums.ClientType.Individual
                };
                db.Clients.Add(walkIn);
                await db.SaveChangesAsync(ct);
            }

            // Patch the ticket itself so future operations use the real client
            ticket.ClientId  = walkIn.Id;
            resolvedClientId = walkIn.Id;
        }

        // ── Step 9: create the Sale ──────────────────────────────────────────────────
        var sale = new Sale
        {
            CompanyId         = request.CompanyId,
            LocationId        = ticket.LocationId,
            ClientId          = resolvedClientId,
            TransactionNumber = txNumber,
            Source            = SaleSource.Ticket,
            TicketId          = ticket.Id,
            SubTotal          = subTotal,
            DiscountAmount    = discountTotal,
            TaxAmount         = taxAmount,
            TotalAmount       = saleTotal,
            Status            = status,
            Items             = saleItems,
            History           = history,
        };

        db.Sales.Add(sale);
        await db.SaveChangesAsync(ct);  // ← sale.Id is now set

        // ── Step 10: record final payment (linked directly to Sale) ──────────────────
        if (finalAmount > 0)
        {
            db.Payments.Add(new Payment
            {
                CompanyId         = request.CompanyId,
                SaleId            = sale.Id,
                TicketId          = ticket.Id,
                Amount            = finalAmount,
                Method            = finalMethod,
                Purpose           = PaymentPurpose.Final,
                AccountId         = finalAccountId,
                ExternalReference = request.ExternalReference,
                Comment           = request.Comment,
                PaidAt            = dateTime.UtcNow,
            });
        }

        // ── Step 11: link advance payments to Sale ───────────────────────────────────
        foreach (var p in advancePayments)
            p.SaleId = sale.Id;

        // ── Step 12: finalise ticket ─────────────────────────────────────────────────
        ticket.Status   = TicketStatus.Done;
        ticket.ClosedAt = dateTime.UtcNow;
        ticket.SaleId   = sale.Id;

        if (ticket.Financials is not null)
        {
            if (status == SaleStatus.Completed)
                ticket.Financials.IsPaid = true;

            if (finalAmount > 0)
            {
                // Reflect the final payment in Financials.Prepayment total
                ticket.Financials.Prepayment      += finalAmount;
                ticket.Financials.PrepaymentMethod = finalMethod.ToString();
                ticket.Financials.PrepaymentPaidAt = dateTime.UtcNow;
            }
        }

        db.ActivityLogs.Add(new ActivityLog
        {
            CompanyId  = request.CompanyId,
            EntityType = ActivityEntityType.Ticket,
            EntityId   = ticket.Id,
            Action     = "Collected",
            OldValue   = ticketInfo.Status.ToString(),
            NewValue   = TicketStatus.Done.ToString(),
            Details    = System.Text.Json.JsonSerializer.Serialize(new { notes = $"Sale {txNumber} created. Total paid: {totalPaid:C}." }),
        });

        await db.SaveChangesAsync(ct);

        return Result<Guid>.Success(sale.Id);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Mirrors the same logic in RecordTicketAdvanceCommandHandler.
    // Must be called BEFORE any ticket/payment entities enter the change tracker.
    // ─────────────────────────────────────────────────────────────────────────────
    private async Task<Guid> ResolvePaymentAccountAsync(
        Guid companyId, Guid locationId, PaymentMethod method, CancellationToken ct)
    {
        var mapping = await db.PaymentMethodMappings
            .FirstOrDefaultAsync(m =>
                m.CompanyId == companyId && m.LocationId == locationId && m.Method == method, ct);

        if (mapping is not null) return mapping.PaymentAccountId;

        var accountType = method switch
        {
            PaymentMethod.Cash       => PaymentAccountType.Cash,
            PaymentMethod.Card       => PaymentAccountType.Bank,
            PaymentMethod.NetBanking => PaymentAccountType.Bank,
            _                        => PaymentAccountType.Gateway,
        };

        var account = await db.PaymentAccounts
            .FirstOrDefaultAsync(a => a.CompanyId == companyId && a.Type == accountType && a.IsActive, ct);

        if (account is null)
        {
            account = new PaymentAccount { CompanyId = companyId, Name = method.ToString(), Type = accountType, IsActive = true };
            db.PaymentAccounts.Add(account);
            await db.SaveChangesAsync(ct);
        }

        db.PaymentMethodMappings.Add(new PaymentMethodMapping
        {
            CompanyId = companyId, LocationId = locationId,
            Method = method, PaymentAccountId = account.Id,
        });
        await db.SaveChangesAsync(ct);

        return account.Id;
    }
}

