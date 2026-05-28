using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Queries.GetPayments;

public class GetPaymentsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPaymentsQuery, Result<PaginatedList<PaymentListItemDto>>>
{
    // Plain class (not file-scoped record) — EF Core SqlQuery<T> nav-expander
    // requires a stable, non-mangled type name to build its internal entity model.
    private sealed class TicketRow { public Guid Id { get; set; } public string? TicketNumber { get; set; } }

    public async Task<Result<PaginatedList<PaymentListItemDto>>> Handle(
        GetPaymentsQuery request, CancellationToken ct)
    {
        var query = db.Payments
            .Include(p => p.Account)
            .Where(p => p.CompanyId == request.CompanyId);

        // Filter by store — join through Sale.LocationId and ServiceTicket.LocationId
        if (request.LocationId.HasValue)
        {
            var locId = request.LocationId.Value;

            // Materialise to List<Guid> — keeping this as IQueryable<Guid> causes
            // EF Core's NavigationExpandingExpressionVisitor to traverse the Sales
            // entity's navigations inside the outer query, producing IndexOutOfRange.
            var saleIdsAtLocation = await db.Sales
                .Where(s => s.CompanyId == request.CompanyId && s.LocationId == locId)
                .Select(s => s.Id)
                .ToListAsync(ct);

            // ServiceTickets table only exists in Repair-industry databases.
            // Check active modules before issuing the raw SQL so Food/Hotel/Medical/Jewelry never crash.
            var tenant = await db.Tenants
                .FirstOrDefaultAsync(t => t.Id == request.CompanyId, ct);
            var hasTickets = tenant?.ActiveModules != null
                && tenant.ActiveModules.Split(',').Select(m => m.Trim())
                   .Contains("ServiceTickets", StringComparer.OrdinalIgnoreCase);

            List<Guid> ticketIdsAtLocation = [];
            if (hasTickets)
            {
                var companyId = request.CompanyId;
                ticketIdsAtLocation = await db.Database.SqlQuery<Guid>($"""
                    SELECT "Id" AS "Value"
                    FROM   "ServiceTickets"
                    WHERE  "CompanyId"  = {companyId}
                      AND  "LocationId" = {locId}
                      AND  "IsActive"   = true
                    """).ToListAsync(ct);
            }

            query = query.Where(p =>
                (p.SaleId   != null && saleIdsAtLocation.Contains(p.SaleId.Value)) ||
                (p.TicketId != null && ticketIdsAtLocation.Contains(p.TicketId.Value)));
        }

        if (request.TicketId.HasValue)
            query = query.Where(p => p.TicketId == request.TicketId.Value);

        if (request.SaleId.HasValue)
            query = query.Where(p => p.SaleId == request.SaleId.Value);

        if (!string.IsNullOrWhiteSpace(request.Method) &&
            Enum.TryParse<PaymentMethod>(request.Method, true, out var method))
            query = query.Where(p => p.Method == method);

        if (!string.IsNullOrWhiteSpace(request.Purpose) &&
            Enum.TryParse<PaymentPurpose>(request.Purpose, true, out var purpose))
            query = query.Where(p => p.Purpose == purpose);

        if (request.DateFrom.HasValue)
            query = query.Where(p => p.PaidAt >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(p => p.PaidAt <= request.DateTo.Value);

        var total = await query.CountAsync(ct);

        var payments = await query
            .OrderByDescending(p => p.PaidAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        // ── Batch-load ticket numbers ─────────────────────────────────────────────
        var ticketIds = payments
            .Where(p => p.TicketId.HasValue)
            .Select(p => p.TicketId!.Value)
            .Distinct()
            .ToList();

        // Ticket numbers are Repair-only. If ticketIds is non-empty we know this is Repair,
        // because non-Repair payments never set TicketId. Use SqlQuery with array parameter
        // to avoid string-concatenation injection and fix PostgreSQL identifier quoting.
        var ticketNumbers = new Dictionary<Guid, string>();
        if (ticketIds.Count > 0)
        {
            var idArray = ticketIds.ToArray();
            var rows = await db.Database
                .SqlQuery<TicketRow>($"""
                    SELECT "Id", "TicketNumber"
                    FROM   "ServiceTickets"
                    WHERE  "Id" = ANY({idArray})
                    """)
                .ToListAsync(ct);

            foreach (var row in rows)
                ticketNumbers[row.Id] = row.TicketNumber ?? string.Empty;
        }

        // ── Batch-load sale transaction numbers ───────────────────────────────────
        var saleIds = payments
            .Where(p => p.SaleId.HasValue)
            .Select(p => p.SaleId!.Value)
            .Distinct()
            .ToList();

        var saleTxNumbers = saleIds.Count > 0
            ? await db.Sales
                .Where(s => saleIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.TransactionNumber ?? string.Empty, ct)
            : new Dictionary<Guid, string>();

        var items = payments.Select(p => new PaymentListItemDto(
            p.Id,
            p.Amount,
            p.Method.ToString(),
            p.Purpose.ToString(),
            p.Account?.Name ?? p.Method.ToString(),
            p.Account?.Type.ToString() ?? string.Empty,
            p.TicketId,
            p.TicketId.HasValue && ticketNumbers.TryGetValue(p.TicketId.Value, out var tn) ? tn : null,
            p.SaleId,
            p.SaleId.HasValue && saleTxNumbers.TryGetValue(p.SaleId.Value, out var sn) ? sn : null,
            p.ExternalReference,
            p.Comment,
            p.PaidAt
        )).ToList();

        return Result<PaginatedList<PaymentListItemDto>>.Success(
            new PaginatedList<PaymentListItemDto>(items, total, request.Page, request.PageSize));
    }
}
