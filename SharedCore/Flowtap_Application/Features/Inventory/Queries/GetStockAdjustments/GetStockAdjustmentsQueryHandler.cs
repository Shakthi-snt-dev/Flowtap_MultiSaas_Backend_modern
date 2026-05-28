using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetStockAdjustments;

public class GetStockAdjustmentsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetStockAdjustmentsQuery, Result<PaginatedList<StockAdjustmentDto>>>
{
    public async Task<Result<PaginatedList<StockAdjustmentDto>>> Handle(GetStockAdjustmentsQuery request, CancellationToken ct)
    {
        var query = db.StockAdjustments
            .Include(a => a.Product)
            .Where(a => a.CompanyId == request.CompanyId);

        if (request.WarehouseId.HasValue) query = query.Where(a => a.WarehouseId == request.WarehouseId.Value);
        if (request.ProductId.HasValue)   query = query.Where(a => a.ProductId == request.ProductId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new StockAdjustmentDto(
                a.Id, a.ProductId,
                a.Product != null ? a.Product.Name : string.Empty,
                a.WarehouseId, a.AdjustmentNumber, a.AdjustmentType.ToString(),
                a.QuantityDifference, a.QuantityBefore, a.QuantityAfter,
                a.Reason, a.IsApproved, a.CreatedAt))
            .ToListAsync(ct);

        return Result<PaginatedList<StockAdjustmentDto>>.Success(
            new PaginatedList<StockAdjustmentDto>(items, total, request.Page, request.PageSize));
    }
}
