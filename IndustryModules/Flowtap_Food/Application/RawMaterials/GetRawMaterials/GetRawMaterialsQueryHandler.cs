using Flowtap_Application.Common.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Food.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.RawMaterials.GetRawMaterials;

public class GetRawMaterialsQueryHandler(IFoodDbContext db)
    : IRequestHandler<GetRawMaterialsQuery, Result<List<RawMaterialDto>>>
{
    public async Task<Result<List<RawMaterialDto>>> Handle(GetRawMaterialsQuery request, CancellationToken ct)
    {
        var rawMaterials = await db.Products
            .Where(p => p.CompanyId == request.CompanyId
                     && p.Kind == ProductKind.RawMaterial
                     && p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.SKU,
                p.DefaultCostPrice,
                Stock = request.WarehouseId.HasValue
                    ? db.WarehouseStocks
                        .Where(s => s.ProductId == p.Id && s.WarehouseId == request.WarehouseId.Value)
                        .Select(s => (decimal?)s.Quantity)
                        .FirstOrDefault()
                    : null
            })
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        var result = rawMaterials
            .Select(p => new RawMaterialDto(p.Id, p.Name, p.SKU, p.Stock, null, p.DefaultCostPrice))
            .ToList();

        return Result<List<RawMaterialDto>>.Success(result);
    }
}
