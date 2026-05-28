using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateProductCategory;

public record ProductCategoryDto(
    Guid Id,
    Guid CompanyId,
    Guid? ParentCategoryId,
    string Name,
    string? IconUrl,
    string? Color,
    int SortOrder,
    bool IsSubCategoryExist,
    bool IsDirectProductExist,
    bool IsBrandExist,
    bool IsActive);

public record CreateProductCategoryCommand(
    Guid CompanyId,
    Guid? ParentCategoryId,
    string Name,
    string? IconUrl,
    string? Color,
    int SortOrder,
    bool IsSubCategoryExist = false,
    bool IsDirectProductExist = true,
    bool IsBrandExist = false) : IRequest<Result<ProductCategoryDto>>;
