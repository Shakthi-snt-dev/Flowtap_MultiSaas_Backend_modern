using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Reports.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Reports.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var salesQuery = db.Sales.Where(s => s.CompanyId == request.CompanyId);
        if (request.LocationId.HasValue)
            salesQuery = salesQuery.Where(s => s.LocationId == request.LocationId.Value);

        var revenueToday = await salesQuery
            .Where(s => s.CreatedAt >= today)
            .SumAsync(s => (decimal?)s.TotalAmount, ct) ?? 0;

        var transactionsToday = await salesQuery
            .CountAsync(s => s.CreatedAt >= today, ct);

        var revenueThisMonth = await salesQuery
            .Where(s => s.CreatedAt >= monthStart)
            .SumAsync(s => (decimal?)s.TotalAmount, ct) ?? 0;

        var newClients = await db.Clients
            .CountAsync(c => c.CompanyId == request.CompanyId && c.CreatedAt >= monthStart, ct);

        // ServiceTickets only exist in the Repair industry — check active modules before querying
        // so Food / Hotel / Medical / Jewelry APIs (which have no servicetickets table) never crash.
        // IApplicationDbContext deliberately has no ServiceTickets DbSet (Repair module owns it),
        // so we use raw SQL — only when confirmed safe by the module flag.
        var tenant = await db.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.CompanyId, ct);

        var hasTickets = tenant?.ActiveModules != null
            && tenant.ActiveModules.Split(',').Select(m => m.Trim())
               .Contains("ServiceTickets", StringComparer.OrdinalIgnoreCase);

        int openTickets = 0;
        if (hasTickets)
        {
            // PostgreSQL requires double-quoted identifiers (EF Core migrations create them with PascalCase).
            // SqlQuery<int> reads a column named "Value"; COUNT(*)::int casts bigint → int for PostgreSQL.
            var companyId  = request.CompanyId;
            var locationId = request.LocationId;

            openTickets = locationId.HasValue
                ? await db.Database.SqlQuery<int>($"""
                    SELECT COUNT(*)::int AS "Value"
                    FROM   "ServiceTickets"
                    WHERE  "CompanyId"  = {companyId}
                      AND  "LocationId" = {locationId.Value}
                      AND  "IsActive"   = true
                      AND  "ClosedAt"   IS NULL
                    """).FirstOrDefaultAsync(ct)
                : await db.Database.SqlQuery<int>($"""
                    SELECT COUNT(*)::int AS "Value"
                    FROM   "ServiceTickets"
                    WHERE  "CompanyId" = {companyId}
                      AND  "IsActive"  = true
                      AND  "ClosedAt"  IS NULL
                    """).FirstOrDefaultAsync(ct);
        }

        var lowStockAlerts = await db.ReorderAlerts
            .CountAsync(a => a.CompanyId == request.CompanyId && !a.IsHandled, ct);

        var stats = new DashboardStatsDto(
            revenueToday, transactionsToday, newClients,
            openTickets, lowStockAlerts, revenueThisMonth);

        return Result<DashboardStatsDto>.Success(stats);
    }
}
