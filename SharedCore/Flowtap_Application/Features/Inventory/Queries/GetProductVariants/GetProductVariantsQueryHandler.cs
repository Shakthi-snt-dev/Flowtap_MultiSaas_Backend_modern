using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetProductVariants;

public class GetProductVariantsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetProductVariantsQuery, Result<List<ProductVariantDto>>>
{
    public async Task<Result<List<ProductVariantDto>>> Handle(GetProductVariantsQuery request, CancellationToken ct)
    {
        var items = await db.ProductVariants
            .Where(v => v.ProductId == request.ProductId)
            .OrderBy(v => v.Name)
            .Select(v => new ProductVariantDto(
                v.Id, v.ProductId, v.Name, v.SKU, v.AdditionalPrice, v.IsActive))
            .ToListAsync(ct);

        return Result<List<ProductVariantDto>>.Success(items);
    }
}
