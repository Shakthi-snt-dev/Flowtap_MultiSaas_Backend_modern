using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetProducts;

public class GetProductsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetProductsQuery, Result<PaginatedList<ProductListItemDto>>>
{
    public async Task<Result<PaginatedList<ProductListItemDto>>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var query = db.Products
            .Include(p => p.Media)
            .Include(p => p.Category)
            .Include(p => p.WarehouseStocks)
            .Where(p => p.CompanyId == request.CompanyId);

        if (request.IsActive.HasValue)
            query = query.Where(p => p.IsActive == request.IsActive.Value);

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(p => p.Name.Contains(request.Search) || p.SKU.Contains(request.Search));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(p => p.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        // Batch-load store-specific prices (single query — not N queries)
        var locationPriceMap   = new Dictionary<Guid, decimal>();
        var locationTaxMap     = new Dictionary<Guid, bool>();
        var locationTaxSlabMap = new Dictionary<Guid, Guid>();   // productId → store-specific taxSlabId

        if (request.LocationId.HasValue && request.LocationId.Value != Guid.Empty)
        {
            var productIds = items.Select(p => p.Id).ToList();
            var locationPrices = await db.ProductLocationPrices
                .Where(lp => lp.LocationId == request.LocationId.Value
                          && productIds.Contains(lp.ProductId)
                          && lp.IsActive)
                .ToListAsync(ct);

            foreach (var lp in locationPrices)
            {
                locationPriceMap[lp.ProductId] = lp.SalePrice;
                locationTaxMap[lp.ProductId]   = lp.IsTaxIncluded;
                if (lp.TaxSlabId.HasValue)
                    locationTaxSlabMap[lp.ProductId] = lp.TaxSlabId.Value;
            }
        }

        List<Guid>? locationWarehouseIds = null;
        var activeStoreId = currentUser.StoreId;
        if (activeStoreId.HasValue && activeStoreId.Value != Guid.Empty)
        {
            locationWarehouseIds = await db.Warehouses
                .Where(w => w.LocationId == activeStoreId.Value)
                .Select(w => w.Id)
                .ToListAsync(ct);
        }

        var dtos = items.Select(p =>
        {
            decimal stock;
            if (request.WarehouseId.HasValue)
            {
                stock = p.WarehouseStocks.Where(ws => ws.WarehouseId == request.WarehouseId.Value).Sum(ws => ws.Quantity);
            }
            else if (locationWarehouseIds != null)
            {
                stock = p.WarehouseStocks.Where(ws => locationWarehouseIds.Contains(ws.WarehouseId)).Sum(ws => ws.Quantity);
            }
            else
            {
                stock = p.WarehouseStocks.Sum(ws => ws.Quantity);
            }

            return new ProductListItemDto(
                p.Id, p.Name, p.SKU, p.Kind.ToString(), p.DefaultSalePrice,
                p.IsActive, p.PublishStatus.ToString(),
                p.Media.FirstOrDefault(m => m.IsPrimary)?.Url,
                p.CategoryId, p.Category?.Name,
                stock,
                p.TaxSlabId,
                locationPriceMap.TryGetValue(p.Id, out var lsp) ? lsp : null,
                locationTaxMap.TryGetValue(p.Id, out var lit) ? lit : null,
                locationTaxSlabMap.TryGetValue(p.Id, out var ltsi) ? ltsi : null);
        }).ToList();

        return Result<PaginatedList<ProductListItemDto>>.Success(
            new PaginatedList<ProductListItemDto>(dtos, total, request.Page, request.PageSize));
    }
}
