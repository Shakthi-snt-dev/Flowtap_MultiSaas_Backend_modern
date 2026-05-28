using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateProductCategory;

public record UpdateProductCategoryCommand(
    Guid Id, Guid CompanyId,
    string? Name, string? IconUrl, string? Color,
    int? SortOrder, bool? IsActive) : IRequest<Result<bool>>;
