using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Reports.DTOs;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Reports.Queries.GetAdminOverview;

public class GetAdminOverviewQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAdminOverviewQuery, Result<AdminOverviewDto>>
{
    public async Task<Result<AdminOverviewDto>> Handle(GetAdminOverviewQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var todayStart  = now.Date;
        var monthStart  = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        // ── Tenant ──────────────────────────────────────────────────────────
        var tenant = await db.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.CompanyId, ct);

        if (tenant is null)
            return Result<AdminOverviewDto>.Failure("Tenant not found.");

        var modules = (tenant.ActiveModules ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(m => m.Trim())
            .ToList();

        // ── Stores ──────────────────────────────────────────────────────────
        var stores = await db.Stores
            .Where(s => s.CompanyId == request.CompanyId && s.IsActive)
            .ToListAsync(ct);

        var storeIds = stores.Select(s => s.Id).ToList();

        // Per-store employee counts (group by DefaultLocationId)
        var employeeCountsByStore = await db.Employees
            .Where(e => e.CompanyId == request.CompanyId && e.Status == EmployeeStatus.Active)
            .GroupBy(e => e.DefaultLocationId)
            .Select(g => new { LocationId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        // Per-store stock item counts (distinct products per store via Warehouse.LocationId)
        var stockCountsByStore = await db.WarehouseStocks
            .Where(ws => ws.CompanyId == request.CompanyId && ws.Quantity > 0)
            .Join(db.Warehouses,
                  ws => ws.WarehouseId,
                  w  => w.Id,
                  (ws, w) => new { w.LocationId, ws.ProductId })
            .Where(x => x.LocationId != null && storeIds.Contains(x.LocationId!.Value))
            .GroupBy(x => x.LocationId)
            .Select(g => new { LocationId = g.Key, Count = g.Select(x => x.ProductId).Distinct().Count() })
            .ToListAsync(ct);

        // Per-store revenue — load ALL company sales (no location filter) then aggregate in-memory.
        // Filtering by storeIds in the DB query would silently drop sales whose LocationId is
        // Guid.Empty (sales made when no store was selected at the POS) or a retired location.
        var rawStoreSales = await db.Sales
            .Where(s => s.CompanyId == request.CompanyId)
            .Select(s => new { s.LocationId, s.TotalAmount, s.CreatedAt })
            .ToListAsync(ct);

        var salesByStore = rawStoreSales
            .GroupBy(s => s.LocationId)
            .Select(g => new
            {
                LocationId       = g.Key,
                RevenueAllTime   = g.Sum(s => s.TotalAmount),
                RevenueThisMonth = g.Where(s => s.CreatedAt >= monthStart).Sum(s => s.TotalAmount),
                RevenueToday     = g.Where(s => s.CreatedAt >= todayStart).Sum(s => s.TotalAmount),
                SalesCount       = g.Count(),
            })
            .ToList();

        // Country → currency fallback map (mirrors the frontend COUNTRY_CURRENCY constant)
        var countryCurrencyFallback = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["US"] = "USD", ["CA"] = "CAD", ["GB"] = "GBP", ["IN"] = "INR",
            ["AE"] = "AED", ["AU"] = "AUD", ["PK"] = "PKR", ["NG"] = "NGN", ["SG"] = "SGD",
        };

        var storeOverviews = stores.Select(s =>
        {
            var rev         = salesByStore.FirstOrDefault(r => r.LocationId == s.Id);
            var countryCode = s.CountryCode  ?? string.Empty;
            var currencyCode = (s.CurrencyCode is { Length: > 0 } cc)
                ? cc
                : countryCurrencyFallback.GetValueOrDefault(countryCode, "USD");

            return new StoreOverviewDto(
                s.Id,
                s.Title,
                countryCode,
                currencyCode,
                employeeCountsByStore.FirstOrDefault(e => e.LocationId == s.Id)?.Count ?? 0,
                stockCountsByStore.FirstOrDefault(x => x.LocationId == s.Id)?.Count ?? 0,
                s.IsActive,
                rev?.RevenueToday     ?? 0,
                rev?.RevenueThisMonth ?? 0,
                rev?.RevenueAllTime   ?? 0,
                rev?.SalesCount       ?? 0);
        }).ToList();

        // ── Aggregates ──────────────────────────────────────────────────────
        var totalEmployees = await db.Employees
            .CountAsync(e => e.CompanyId == request.CompanyId && e.Status == EmployeeStatus.Active, ct);

        var totalProducts = await db.Products
            .CountAsync(p => p.CompanyId == request.CompanyId && p.IsActive, ct);

        var totalStockQty = await db.WarehouseStocks
            .Where(ws => ws.CompanyId == request.CompanyId)
            .SumAsync(ws => (decimal?)ws.Quantity, ct) ?? 0;

        var totalClients = await db.Clients
            .CountAsync(c => c.CompanyId == request.CompanyId && c.IsActive, ct);

        // ServiceTickets table only exists in Repair-industry databases.
        // Guard with module check so Food/Hotel/Medical/Jewelry APIs never crash.
        int openTickets = 0;
        if (modules.Contains("ServiceTickets", StringComparer.OrdinalIgnoreCase))
        {
            var companyId = request.CompanyId;
            openTickets = await db.Database.SqlQuery<int>($"""
                SELECT COUNT(*)::int AS "Value"
                FROM   "ServiceTickets"
                WHERE  "CompanyId" = {companyId}
                  AND  "IsActive"  = true
                  AND  "ClosedAt"  IS NULL
                """).FirstOrDefaultAsync(ct);
        }

        // ── Revenue ─────────────────────────────────────────────────────────
        var salesBase = db.Sales.Where(s => s.CompanyId == request.CompanyId);

        var revenueToday = await salesBase
            .Where(s => s.CreatedAt >= todayStart)
            .SumAsync(s => (decimal?)s.TotalAmount, ct) ?? 0;

        var revenueThisMonth = await salesBase
            .Where(s => s.CreatedAt >= monthStart)
            .SumAsync(s => (decimal?)s.TotalAmount, ct) ?? 0;

        var revenueAllTime = await salesBase
            .SumAsync(s => (decimal?)s.TotalAmount, ct) ?? 0;

        var totalSalesCount = await salesBase.CountAsync(ct);

        // ── Subscription ────────────────────────────────────────────────────
        var sub = await db.CompanySubscriptions
            .Include(s => s.SubscriptionPlan)
            .Where(s => s.CompanyId == request.CompanyId && s.IsActive)
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefaultAsync(ct);

        SubscriptionSummaryDto? subscriptionSummary = sub is null ? null : new SubscriptionSummaryDto(
            sub.SubscriptionPlan?.Name ?? "Free",
            sub.Status.ToString(),
            sub.StartDate,
            sub.EndDate,
            sub.NextBillingDate,
            sub.TotalLocations);

        // ── Result ──────────────────────────────────────────────────────────
        var dto = new AdminOverviewDto(
            TenantName:        tenant.Title,
            BusinessType:      tenant.BusinessType ?? string.Empty,
            Country:           tenant.Country ?? string.Empty,
            Currency:          tenant.Currency ?? string.Empty,
            ActiveModules:     modules,
            TotalStores:       stores.Count,
            TotalEmployees:    totalEmployees,
            TotalProducts:     totalProducts,
            TotalStockQuantity: totalStockQty,
            TotalClients:      totalClients,
            OpenTickets:       openTickets,
            RevenueToday:      revenueToday,
            RevenueThisMonth:  revenueThisMonth,
            RevenueAllTime:    revenueAllTime,
            TotalSalesCount:   totalSalesCount,
            Stores:            storeOverviews,
            Subscription:      subscriptionSummary);

        return Result<AdminOverviewDto>.Success(dto);
    }
}

