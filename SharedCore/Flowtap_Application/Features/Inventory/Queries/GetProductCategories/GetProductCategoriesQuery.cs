using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.Commands.CreateProductCategory;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetProductCategories;

public record GetProductCategoriesQuery(
    Guid CompanyId,
    Guid? ParentCategoryId,
    bool? IsActive,
    bool IncludeSubCategories = false) : IRequest<Result<List<ProductCategoryDto>>>;
