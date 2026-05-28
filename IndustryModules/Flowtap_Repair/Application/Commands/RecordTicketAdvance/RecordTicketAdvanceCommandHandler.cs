using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Repair.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Commands.RecordTicketAdvance;

public class RecordTicketAdvanceCommandHandler(IRepairDbContext db, IDateTimeService dateTime)
    : IRequestHandler<RecordTicketAdvanceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RecordTicketAdvanceCommand request, CancellationToken ct)
    {
        if (request.Amount <= 0)
            return Result<Guid>.Failure("Advance amount must be greater than zero.");

        if (!Enum.TryParse<PaymentMethod>(request.Method, true, out var method))
            return Result<Guid>.Failure($"Invalid payment method: {request.Method}");

        // ── Step 1: lightweight projection — get LocationId without tracking ticket ──
        // ResolvePaymentAccountAsync may SaveChangesAsync internally. Having the tracked
        // ticket in the change tracker would cause EF relationship-fixup issues.
        var ticketInfo = await db.ServiceTickets
            .Where(t => t.Id == request.TicketId && t.CompanyId == request.CompanyId)
            .Select(t => new { t.LocationId })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(nameof(ServiceTicket), request.TicketId);

        // ── Step 2: resolve payment account (intermediate SaveChangesAsync safe here) ──
        var accountId = request.AccountId ?? Guid.Empty;
        if (accountId == Guid.Empty)
        {
            accountId = await ResolvePaymentAccountAsync(
                request.CompanyId, ticketInfo.LocationId, method, ct);
        }

        // ── Step 3: load ticket with tracking ────────────────────────────────────────
        var ticket = await db.ServiceTickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException(nameof(ServiceTicket), request.TicketId);

        // ── Step 4: create Payment linked to Ticket (not to a Sale) ─────────────────
        var payment = new Payment
        {
            CompanyId         = request.CompanyId,
            TicketId          = request.TicketId,   // linked to ticket, SaleId = null
            SaleId            = null,
            Amount            = request.Amount,
            Method            = method,
            Purpose           = PaymentPurpose.Advance,
            AccountId         = accountId,
            ExternalReference = request.ExternalReference,
            Comment           = request.Comment,
            EmployeeId        = request.EmployeeId,
            PaidAt            = dateTime.UtcNow,
        };
        db.Payments.Add(payment);

        // ── Step 5: update ticket financials ─────────────────────────────────────────
        if (ticket.Financials is null)
            ticket.Financials = new ServiceFinancials();

        ticket.Financials.Prepayment     += request.Amount;
        ticket.Financials.PrepaymentMethod = request.Method;
        ticket.Financials.PrepaymentPaidAt = dateTime.UtcNow;

        // ── Step 6: single SaveChangesAsync ──────────────────────────────────────────
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(payment.Id);
    }

    /// <summary>
    /// Resolves the PaymentAccount for a given method at a given location.
    /// Mirrors the same logic in CreateSaleCommandHandler / AddPaymentCommandHandler.
    /// Must be called BEFORE any ticket/payment entities enter the change tracker.
    /// </summary>
    private async Task<Guid> ResolvePaymentAccountAsync(
        Guid companyId, Guid locationId, PaymentMethod method, CancellationToken ct)
    {
        // 1. Prefer explicit PaymentMethodMapping for this location
        var mapping = await db.PaymentMethodMappings
            .FirstOrDefaultAsync(m =>
                m.CompanyId == companyId &&
                m.LocationId == locationId &&
                m.Method == method, ct);

        if (mapping is not null) return mapping.PaymentAccountId;

        // 2. Fall back to any matching active account for this company
        var accountType = method switch
        {
            PaymentMethod.Cash       => PaymentAccountType.Cash,
            PaymentMethod.Card       => PaymentAccountType.Bank,
            PaymentMethod.NetBanking => PaymentAccountType.Bank,
            _                        => PaymentAccountType.Gateway, // UPI, Wallet
        };

        var account = await db.PaymentAccounts
            .FirstOrDefaultAsync(a =>
                a.CompanyId == companyId &&
                a.Type == accountType &&
                a.IsActive, ct);

        // 3. Auto-create account on first run (no mapping configured yet)
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

        // 4. Create mapping so future calls skip this logic
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

