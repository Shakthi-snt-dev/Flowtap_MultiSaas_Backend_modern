using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetProducts;

public record GetProductsQuery(
    Guid CompanyId,
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null,        // null = all, true = active only, false = inactive only
    Guid? CategoryId = null,      // filter by category
    Guid? WarehouseId = null,     // if set, stock shown for this warehouse only; else sum all
    Guid? LocationId = null,      // if set, include store-specific price in response
    string? Kind = null)          // optional ProductKind filter ("FinalProduct" for food POS); null = no filter
    : IRequest<Result<PaginatedList<ProductListItemDto>>>;
