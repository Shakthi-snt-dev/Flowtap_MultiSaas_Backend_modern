using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.AddPayment;

public class AddPaymentCommandHandler(IApplicationDbContext db, IDateTimeService dateTime)
    : IRequestHandler<AddPaymentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddPaymentCommand request, CancellationToken ct)
    {
        // ── Idempotency ───────────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            var dup = await db.Payments
                .FirstOrDefaultAsync(p => p.IdempotencyKey == request.IdempotencyKey
                    && p.CompanyId == request.CompanyId, ct);
            if (dup is not null) return Result<Guid>.Success(dup.Id);
        }

        if (!Enum.TryParse<PaymentMethod>(request.Method, true, out var method))
            return Result<Guid>.Failure($"Invalid payment method: {request.Method}");

        if (!Enum.TryParse<PaymentPurpose>(request.Purpose, true, out var purpose))
            purpose = PaymentPurpose.Final;

        // ── Step 1: load sale info + sum of existing payments as a projection.
        // AsNoTracking: we never modify the Sale entity directly — doing so dragged
        // it into SaveChangesAsync and EF's RowVersion (IsRowVersion() bytea column)
        // caused a spurious DbUpdateConcurrencyException because the tracked NULL value
        // did not match the non-null value stored in the database.
        var saleInfo = await db.Sales
            .AsNoTracking()
            .Where(s => s.Id == request.SaleId && s.CompanyId == request.CompanyId)
            .Select(s => new
            {
                s.LocationId,
                s.TotalAmount,
                PaidSoFar = s.Payments.Sum(p => p.Amount),
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(nameof(Sale), request.SaleId);

        // ── Step 2: resolve account (may call SaveChangesAsync internally) ────
        var accountId = request.AccountId;
        if (accountId == Guid.Empty)
        {
            accountId = await ResolvePaymentAccountAsync(
                request.CompanyId, saleInfo.LocationId, method, ct);
        }

        // ── Step 3: compute totals ────────────────────────────────────────────
        var totalPaid   = saleInfo.PaidSoFar + request.Amount;
        var isFullyPaid = totalPaid >= saleInfo.TotalAmount;

        // ── Step 4: insert payment directly via DbSet ─────────────────────────
        var payment = new Payment
        {
            CompanyId         = request.CompanyId,
            SaleId            = request.SaleId,
            Amount            = request.Amount,
            Method            = method,
            Purpose           = purpose,
            AccountId         = accountId,
            ExternalReference = request.ExternalReference,
            Comment           = request.Comment,
            EmployeeId        = request.EmployeeId,
            PaidAt            = dateTime.UtcNow,
            IdempotencyKey    = request.IdempotencyKey,
        };
        db.Payments.Add(payment);

        // ── Step 5: insert history entries directly via DbSet ─────────────────
        // Directly using db.SaleHistories (not via sale.History collection) avoids
        // EF loading + re-saving the existing history rows as spurious UPDATEs.
        db.SaleHistories.Add(new SaleHistory
        {
            SaleId    = request.SaleId,
            Message   = $"Payment of {request.Amount:C} received via {method} ({purpose}).",
            CreatedAt = dateTime.UtcNow,
        });

        if (isFullyPaid)
        {
            db.SaleHistories.Add(new SaleHistory
            {
                SaleId    = request.SaleId,
                Message   = "Sale completed — fully paid.",
                CreatedAt = dateTime.UtcNow,
            });
        }

        // ── Step 6: SaveChangesAsync — only Payment + SaleHistory, no Sale entity ──
        // No Sale in the change tracker → no RowVersion WHERE clause → no concurrency error.
        await db.SaveChangesAsync(ct);

        // ── Step 7: update Sale status via ExecuteUpdateAsync (bypasses RowVersion) ──
        if (isFullyPaid)
        {
            await db.Sales
                .Where(s => s.Id == request.SaleId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, SaleStatus.Completed)
                    .SetProperty(x => x.UpdatedAt, dateTime.UtcNow), ct);
        }

        return Result<Guid>.Success(payment.Id);
    }

    private async Task<Guid> ResolvePaymentAccountAsync(
        Guid companyId, Guid locationId, PaymentMethod method, CancellationToken ct)
    {
        var mapping = await db.PaymentMethodMappings
            .FirstOrDefaultAsync(m =>
                m.CompanyId == companyId &&
                m.LocationId == locationId &&
                m.Method == method, ct);

        if (mapping is not null) return mapping.PaymentAccountId;

        var accountType = method switch
        {
            PaymentMethod.Cash       => PaymentAccountType.Cash,
            PaymentMethod.Card       => PaymentAccountType.Bank,
            PaymentMethod.NetBanking => PaymentAccountType.Bank,
            _                        => PaymentAccountType.Gateway,
        };

        var account = await db.PaymentAccounts
            .FirstOrDefaultAsync(a =>
                a.CompanyId == companyId &&
                a.Type == accountType &&
                a.IsActive, ct);

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
}
