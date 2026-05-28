using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetStockBatches;

public class GetStockBatchesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetStockBatchesQuery, Result<List<StockBatchDto>>>
{
    public async Task<Result<List<StockBatchDto>>> Handle(GetStockBatchesQuery request, CancellationToken ct)
    {
        var query = db.StockBatches
            .Include(b => b.Product)
            .Include(b => b.Warehouse)
            .Where(b => b.CompanyId == request.CompanyId);

        if (request.ProductId.HasValue)   query = query.Where(b => b.ProductId == request.ProductId.Value);
        if (request.WarehouseId.HasValue) query = query.Where(b => b.WarehouseId == request.WarehouseId.Value);

        var items = await query
            .OrderByDescending(b => b.ReceivedAt)
            .Select(b => new StockBatchDto(
                b.Id, b.ProductId,
                b.Product != null ? b.Product.Name : string.Empty,
                b.WarehouseId,
                b.Warehouse != null ? b.Warehouse.Name : string.Empty,
                b.BatchNumber, b.Quantity, b.CostPrice, b.ReceivedAt, b.ExpiryDate))
            .ToListAsync(ct);

        return Result<List<StockBatchDto>>.Success(items);
    }
}
