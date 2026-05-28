using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.Commands.CreateProductCategory;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetProductCategories;

public class GetProductCategoriesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductCategoriesQuery, Result<List<ProductCategoryDto>>>
{
    public async Task<Result<List<ProductCategoryDto>>> Handle(GetProductCategoriesQuery request, CancellationToken ct)
    {
        var query = context.ProductCategories
            .AsNoTracking()
            .Where(c => c.CompanyId == request.CompanyId);

        if (request.ParentCategoryId.HasValue)
            query = query.Where(c => c.ParentCategoryId == request.ParentCategoryId.Value);

        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);

        if (request.IncludeSubCategories)
            query = query.Include(c => c.SubCategories);

        var categories = await query.ToListAsync(ct);

        var dtos = categories.Select(c => new ProductCategoryDto(
            c.Id,
            c.CompanyId,
            c.ParentCategoryId,
            c.Name,
            c.IconUrl,
            c.Color,
            c.SortOrder,
            c.IsSubCategoryExist,
            c.IsDirectProductExist,
            c.IsBrandExist,
            c.IsActive)).ToList();

        return Result<List<ProductCategoryDto>>.Success(dtos);
    }
}
