using Flowtap_Application.Common.DTOs;
using Flowtap_Food.Application.Reports.DTOs;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.Reports;

public class GetFoodDashboardQueryHandler(IFoodDbContext db)
    : IRequestHandler<GetFoodDashboardQuery, Result<FoodDashboardDto>>
{
    public async Task<Result<FoodDashboardDto>> Handle(GetFoodDashboardQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var tableQuery = db.FoodTables.Where(t => t.CompanyId == request.CompanyId && t.IsActive);
        if (request.LocationId.HasValue)
            tableQuery = tableQuery.Where(t => t.LocationId == request.LocationId.Value);

        var tables = await tableQuery
            .Select(t => new { t.Status })
            .ToListAsync(ct);

        var kotQuery = db.KitchenOrders.Where(k => k.CompanyId == request.CompanyId);
        if (request.LocationId.HasValue)
            kotQuery = kotQuery.Where(k => k.LocationId == request.LocationId.Value);

        var kots = await kotQuery
            .Where(k => k.Status == KOTStatus.New || k.Status == KOTStatus.Ready)
            .Select(k => new { k.Status })
            .ToListAsync(ct);

        var salesQuery = db.Sales.Where(s => s.CompanyId == request.CompanyId
                                          && s.CreatedAt >= today
                                          && s.CreatedAt < tomorrow);
        if (request.LocationId.HasValue)
            salesQuery = salesQuery.Where(s => s.LocationId == request.LocationId.Value);

        var todaySales = await salesQuery
            .Select(s => new { s.TotalAmount, s.CreatedAt })
            .ToListAsync(ct);

        var peakHours = todaySales
            .GroupBy(s => s.CreatedAt.Hour)
            .Select(g => new HourlyOrderCountDto(g.Key, g.Count(), g.Sum(s => s.TotalAmount)))
            .OrderBy(h => h.Hour)
            .ToList();

        return Result<FoodDashboardDto>.Success(new FoodDashboardDto(
            ActiveTables: tables.Count(t => t.Status == FoodTableStatus.Occupied),
            AvailableTables: tables.Count(t => t.Status == FoodTableStatus.Available),
            TotalTables: tables.Count,
            PendingKOTs: kots.Count(k => k.Status == KOTStatus.New),
            ReadyKOTs: kots.Count(k => k.Status == KOTStatus.Ready),
            TodayOrders: todaySales.Count,
            TodayRevenue: todaySales.Sum(s => s.TotalAmount),
            PeakHours: peakHours
        ));
    }
}
