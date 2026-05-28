using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.CreateServiceCategory;

public record ServiceCategoryDto(
    Guid Id,
    Guid CompanyId,
    Guid? ParentCategoryId,
    string Name,
    string? Description,
    string? IconUrl,
    int SortOrder,
    bool IsActive);

public record CreateServiceCategoryCommand(
    Guid CompanyId,
    Guid? ParentCategoryId,
    string Name,
    string? Description,
    string? IconUrl,
    int SortOrder) : IRequest<Result<ServiceCategoryDto>>;

