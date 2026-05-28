using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetProductLocationPrices;

public class GetProductLocationPricesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetProductLocationPricesQuery, Result<List<ProductLocationPriceDto>>>
{
    public async Task<Result<List<ProductLocationPriceDto>>> Handle(GetProductLocationPricesQuery request, CancellationToken ct)
    {
        var items = await db.ProductLocationPrices
            .Where(p => p.CompanyId == request.CompanyId && p.ProductId == request.ProductId && p.IsActive)
            .OrderBy(p => p.LocationId)
            .Select(p => new ProductLocationPriceDto(
                p.Id, p.LocationId, p.CostPrice, p.SalePrice, p.MRP,
                p.IsTaxIncluded, p.Status.ToString(), p.EffectiveFrom))
            .ToListAsync(ct);

        return Result<List<ProductLocationPriceDto>>.Success(items);
    }
}
