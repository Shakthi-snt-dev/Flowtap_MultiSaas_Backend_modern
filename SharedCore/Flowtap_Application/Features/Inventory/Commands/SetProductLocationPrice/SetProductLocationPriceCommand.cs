using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.SetProductLocationPrice;

public record SetProductLocationPriceCommand(
    Guid CompanyId,
    Guid ProductId,
    Guid LocationId,
    decimal CostPrice,
    decimal SalePrice,
    decimal? MRP = null,
    bool IsTaxIncluded = false,
    Guid? TaxSlabId = null) : IRequest<Result<Guid>>;    // store-specific tax slab
