using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetProductVariants;

public record ProductVariantDto(
    Guid Id,
    Guid ProductId,
    string Name,
    string SKU,
    decimal? AdditionalPrice,
    bool IsActive);

public record GetProductVariantsQuery(Guid ProductId) : IRequest<Result<List<ProductVariantDto>>>;
