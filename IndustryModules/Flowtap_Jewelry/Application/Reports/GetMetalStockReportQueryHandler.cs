using Flowtap_Application.Common.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Jewelry.Application.Reports.DTOs;
using Flowtap_Jewelry.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Jewelry.Application.Reports;

public class GetMetalStockReportQueryHandler(IJewelryDbContext db)
    : IRequestHandler<GetMetalStockReportQuery, Result<MetalStockReportDto>>
{
    public async Task<Result<MetalStockReportDto>> Handle(GetMetalStockReportQuery request, CancellationToken ct)
    {
        var stockQuery = db.WarehouseStocks
            .Where(ws => ws.CompanyId == request.CompanyId && ws.Quantity > 0);
        if (request.WarehouseId.HasValue)
            stockQuery = stockQuery.Where(ws => ws.WarehouseId == request.WarehouseId.Value);

        var items = await stockQuery
            .Join(db.Products.Where(p => p.Kind == ProductKind.FinalProduct || p.Kind == ProductKind.Accessory),
                  ws => ws.ProductId, p => p.Id,
                  (ws, p) => new MetalStockDetailDto(
                      p.Id, p.Name, p.SKU,
                      p.Tag ?? "Unknown",
                      ws.Quantity,
                      p.DefaultCostPrice,
                      ws.Quantity * p.DefaultCostPrice))
            .OrderBy(i => i.MetalType).ThenBy(i => i.ProductName)
            .ToListAsync(ct);

        return Result<MetalStockReportDto>.Success(new MetalStockReportDto(items));
    }
}
