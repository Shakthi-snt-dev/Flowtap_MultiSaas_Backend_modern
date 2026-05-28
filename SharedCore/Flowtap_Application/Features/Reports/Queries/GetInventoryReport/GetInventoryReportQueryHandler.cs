using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Reports.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Reports.Queries.GetInventoryReport;

public class GetInventoryReportQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetInventoryReportQuery, Result<InventoryReportDto>>
{
    public async Task<Result<InventoryReportDto>> Handle(GetInventoryReportQuery request, CancellationToken ct)
    {
        var stockQuery = db.WarehouseStocks
            .Include(ws => ws.Product)
            .Include(ws => ws.Warehouse)
            .Where(ws => ws.CompanyId == request.CompanyId);

        if (request.WarehouseId.HasValue)
            stockQuery = stockQuery.Where(ws => ws.WarehouseId == request.WarehouseId.Value);

        var stocks = await stockQuery.ToListAsync(ct);

        var totalProducts = stocks.Select(ws => ws.ProductId).Distinct().Count();
        var lowStockItems = stocks.Count(ws => ws.Quantity > 0 && ws.Quantity <= ws.ReorderLevel);
        var outOfStockItems = stocks.Count(ws => ws.Quantity <= 0);
        var totalValue = stocks.Sum(ws => ws.Quantity * (ws.Product?.DefaultCostPrice ?? 0));

        var items = stocks
            .GroupBy(ws => ws.ProductId)
            .Select(g => new StockSummaryDto(
                g.Key,
                g.First().Product?.Name ?? string.Empty,
                g.First().Product?.SKU ?? string.Empty,
                g.Sum(ws => ws.Quantity),
                g.Sum(ws => ws.Quantity * (ws.Product?.DefaultCostPrice ?? 0))))
            .ToList();

        var report = new InventoryReportDto(totalProducts, lowStockItems, outOfStockItems, totalValue, items);
        return Result<InventoryReportDto>.Success(report);
    }
}
