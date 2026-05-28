using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetProductLocationPrices;

public record ProductLocationPriceDto(
    Guid Id,
    Guid LocationId,
    decimal CostPrice,
    decimal SalePrice,
    decimal? MRP,
    bool IsTaxIncluded,
    string Status,
    DateTime EffectiveFrom);

public record GetProductLocationPricesQuery(Guid CompanyId, Guid ProductId) : IRequest<Result<List<ProductLocationPriceDto>>>;
