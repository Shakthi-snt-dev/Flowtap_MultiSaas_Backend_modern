using Flowtap_Application.Common.DTOs;
using Flowtap_Jewelry.Application.Reports.DTOs;
using Flowtap_Jewelry.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Jewelry.Application.Reports;

public class GetJewelryDashboardQueryHandler(IJewelryDbContext db)
    : IRequestHandler<GetJewelryDashboardQuery, Result<JewelryDashboardDto>>
{
    public async Task<Result<JewelryDashboardDto>> Handle(GetJewelryDashboardQuery request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var salesQuery = db.Sales.Where(s => s.CompanyId == request.CompanyId);
        if (request.LocationId.HasValue)
            salesQuery = salesQuery.Where(s => s.LocationId == request.LocationId.Value);

        var todayOrders  = await salesQuery.CountAsync(s => s.CreatedAt >= today && s.CreatedAt < tomorrow, ct);
        var todayRevenue = await salesQuery.Where(s => s.CreatedAt >= today && s.CreatedAt < tomorrow).SumAsync(s => s.TotalAmount, ct);
        var monthRevenue = await salesQuery.Where(s => s.CreatedAt >= monthStart).SumAsync(s => s.TotalAmount, ct);

        var exchangeQuery = db.MetalExchangeTransactions.Where(e => e.CompanyId == request.CompanyId);
        if (request.LocationId.HasValue)
            exchangeQuery = exchangeQuery.Where(e => e.LocationId == request.LocationId.Value);

        var todayExchanges = await exchangeQuery
            .Where(e => e.CreatedAt >= today && e.CreatedAt < tomorrow)
            .Select(e => new { e.TotalValue })
            .ToListAsync(ct);

        // Metal stock summary from warehouse stocks
        var stockSummary = await db.WarehouseStocks
            .Where(ws => ws.CompanyId == request.CompanyId && ws.Quantity > 0)
            .Join(db.Products, ws => ws.ProductId, p => p.Id, (ws, p) => new { p.Tag, ws.Quantity, p.DefaultCostPrice })
            .Where(x => x.Tag != null)
            .GroupBy(x => x.Tag!)
            .Select(g => new MetalStockSummaryDto(
                g.Key,
                g.Sum(x => x.Quantity),
                g.Sum(x => x.Quantity * x.DefaultCostPrice)))
            .ToListAsync(ct);

        return Result<JewelryDashboardDto>.Success(new JewelryDashboardDto(
            TodayOrders: todayOrders,
            TodayRevenue: todayRevenue,
            MonthRevenue: monthRevenue,
            TodayExchanges: todayExchanges.Count,
            TodayExchangeValue: todayExchanges.Sum(e => e.TotalValue),
            MetalStockSummary: stockSummary
        ));
    }
}
