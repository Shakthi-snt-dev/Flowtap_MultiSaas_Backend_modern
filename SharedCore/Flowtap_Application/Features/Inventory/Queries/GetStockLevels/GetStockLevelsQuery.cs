using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetStockLevels;

public record GetStockLevelsQuery(
    Guid CompanyId, Guid? WarehouseId = null, string? Search = null,
    bool LowStockOnly = false, int Page = 1, int PageSize = 50)
    : IRequest<Result<PaginatedList<StockLevelDto>>>;
