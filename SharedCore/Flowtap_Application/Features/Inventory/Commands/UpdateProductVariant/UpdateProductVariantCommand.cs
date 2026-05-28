using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateProductVariant;

public record UpdateProductVariantCommand(
    Guid Id,
    Guid ProductId,
    string? Name = null,
    string? SKU = null,
    decimal? AdditionalPrice = null,
    bool? IsActive = null) : IRequest<Result<bool>>;
