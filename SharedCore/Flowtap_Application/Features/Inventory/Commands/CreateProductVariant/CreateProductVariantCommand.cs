using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateProductVariant;

public record CreateProductVariantCommand(
    Guid ProductId,
    string Name,
    string? SKU = null,
    decimal? AdditionalPrice = null) : IRequest<Result<Guid>>;
