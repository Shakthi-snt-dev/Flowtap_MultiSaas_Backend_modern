using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.CreateProductCategory;

public class CreateProductCategoryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateProductCategoryCommand, Result<ProductCategoryDto>>
{
    public async Task<Result<ProductCategoryDto>> Handle(CreateProductCategoryCommand request, CancellationToken ct)
    {
        var category = new ProductCategory
        {
            CompanyId = request.CompanyId,
            ParentCategoryId = request.ParentCategoryId,
            Name = request.Name,
            IconUrl = request.IconUrl,
            Color = request.Color,
            SortOrder = request.SortOrder,
            IsSubCategoryExist = request.IsSubCategoryExist,
            IsDirectProductExist = request.IsDirectProductExist,
            IsBrandExist = request.IsBrandExist,
            IsActive = true
        };

        context.ProductCategories.Add(category);

        if (request.ParentCategoryId.HasValue)
        {
            var parent = await context.ProductCategories
                .FirstOrDefaultAsync(p => p.Id == request.ParentCategoryId.Value && p.CompanyId == request.CompanyId, ct);

            if (parent != null)
                parent.IsSubCategoryExist = true;
        }

        await context.SaveChangesAsync(ct);

        return Result<ProductCategoryDto>.Success(new ProductCategoryDto(
            category.Id,
            category.CompanyId,
            category.ParentCategoryId,
            category.Name,
            category.IconUrl,
            category.Color,
            category.SortOrder,
            category.IsSubCategoryExist,
            category.IsDirectProductExist,
            category.IsBrandExist,
            category.IsActive));
    }
}
