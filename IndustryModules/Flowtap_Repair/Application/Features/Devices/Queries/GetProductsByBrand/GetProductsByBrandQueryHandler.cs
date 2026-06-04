using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Repair.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Queries.GetProductsByBrand;

public class GetProductsByBrandQueryHandler(IRepairDbContext db, IApplicationDbContext coreDb)
    : IRequestHandler<GetProductsByBrandQuery, Result<List<ProductListItemDto>>>
{
    public async Task<Result<List<ProductListItemDto>>> Handle(
        GetProductsByBrandQuery request, CancellationToken ct)
    {
        // 1. Find all model IDs belonging to the brand
        //    DeviceModel has no CompanyId — scoped via BrandId (brand already belongs to company)
        var modelIds = await db.DeviceModels
            .Where(m => m.BrandId == request.BrandId && m.IsActive)
            .Select(m => m.Id)
            .ToListAsync(ct);

        if (modelIds.Count == 0)
            return Result<List<ProductListItemDto>>.Success([]);

        // 2. Find product IDs linked to those models via the junction table
        var productIds = await db.ProductDeviceModelMappings
            .Where(m => modelIds.Contains(m.DeviceModelId))
            .Select(m => m.ProductId)
            .Distinct()
            .ToListAsync(ct);

        if (productIds.Count == 0)
            return Result<List<ProductListItemDto>>.Success([]);

        // 3. Load the products with optional kind + category filters
        var query = db.Products
            .Include(p => p.Media)
            .Include(p => p.Category)
            .Include(p => p.WarehouseStocks)
            .Where(p => p.CompanyId == request.CompanyId
                     && p.IsActive
                     && productIds.Contains(p.Id));

        // Category filter — with optional sub-category expansion.
        // Example: categoryId=Mobile + includeSubCategories=true
        //   → finds [Mobile, Android, iPhone] and includes products from all three.
        if (request.CategoryId.HasValue)
        {
            if (request.IncludeSubCategories)
            {
                var childIds = await coreDb.ProductCategories
                    .Where(c => c.ParentCategoryId == request.CategoryId.Value && c.IsActive)
                    .Select(c => c.Id)
                    .ToListAsync(ct);
                var allCategoryIds = childIds.Append(request.CategoryId.Value).ToList();
                query = query.Where(p => allCategoryIds.Contains(p.CategoryId));
            }
            else
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }
        }

        // Kind filter — supports comma-separated e.g. "Device,Accessory"
        if (!string.IsNullOrWhiteSpace(request.Kind))
        {
            var kinds = request.Kind
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(k => Enum.TryParse<ProductKind>(k, ignoreCase: true, out var kf) ? kf : (ProductKind?)null)
                .Where(k => k.HasValue)
                .Select(k => k!.Value)
                .ToList();

            if (kinds.Count == 1)
                query = query.Where(p => p.Kind == kinds[0]);
            else if (kinds.Count > 1)
                query = query.Where(p => kinds.Contains(p.Kind));
        }

        // Project to anonymous type server-side to avoid optional-arg EF expression tree issues,
        // then map to the DTO client-side.
        var raw = await query
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Id, p.Name, p.SKU,
                Kind          = p.Kind.ToString(),
                p.DefaultSalePrice,
                p.IsActive,
                PublishStatus = p.PublishStatus.ToString(),
                PrimaryImage  = p.Media.Where(m => m.IsPrimary).Select(m => m.Url).FirstOrDefault(),
                p.CategoryId,
                CategoryName  = p.Category != null ? p.Category.Name : null,
                Stock         = p.WarehouseStocks.Sum(ws => ws.Quantity),
            })
            .ToListAsync(ct);

        var products = raw
            .Select(p => new ProductListItemDto(
                p.Id, p.Name, p.SKU, p.Kind,
                p.DefaultSalePrice, p.IsActive, p.PublishStatus,
                p.PrimaryImage, p.CategoryId, p.CategoryName, p.Stock))
            .ToList();

        return Result<List<ProductListItemDto>>.Success(products);
    }
}
