using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetStockLevels;

public class GetStockLevelsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetStockLevelsQuery, Result<PaginatedList<StockLevelDto>>>
{
    public async Task<Result<PaginatedList<StockLevelDto>>> Handle(GetStockLevelsQuery request, CancellationToken ct)
    {
        var query = db.WarehouseStocks
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .Where(s => s.CompanyId == request.CompanyId);

        var storeId = currentUser.StoreId;
        if (storeId.HasValue && storeId.Value != Guid.Empty)
        {
            query = query.Where(s => s.Warehouse.LocationId == storeId.Value || s.Warehouse.LocationId == null);
        }

        if (request.WarehouseId.HasValue)
            query = query.Where(s => s.WarehouseId == request.WarehouseId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(s => s.Product.Name.Contains(request.Search) || s.Product.SKU.Contains(request.Search));

        if (request.LowStockOnly)
            query = query.Where(s => s.Quantity <= s.ReorderLevel);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(s => s.Product.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var dtos = items.Select(s => new StockLevelDto(
            s.ProductId, s.Product.Name, s.Product.SKU, s.WarehouseId, s.Warehouse.Name,
            s.Quantity, s.InTransitQuantity, s.ReservedQuantity, s.ReorderLevel)).ToList();

        return Result<PaginatedList<StockLevelDto>>.Success(
            new PaginatedList<StockLevelDto>(dtos, total, request.Page, request.PageSize));
    }
}
