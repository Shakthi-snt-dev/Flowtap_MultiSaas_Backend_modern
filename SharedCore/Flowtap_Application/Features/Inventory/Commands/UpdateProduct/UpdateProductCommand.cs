using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id, Guid CompanyId, string? Name, string? Tag, string? HsnCode,
    decimal? DefaultCostPrice, decimal? DefaultSalePrice,
    bool? IsActive, Guid? TaxSlabId = null) : IRequest<Result<bool>>;
