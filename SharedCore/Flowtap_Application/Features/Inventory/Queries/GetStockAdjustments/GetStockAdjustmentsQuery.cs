using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetStockAdjustments;

public record StockAdjustmentDto(
    Guid Id, Guid ProductId, string ProductName,
    Guid WarehouseId, string AdjustmentNumber, string AdjustmentType,
    decimal QuantityDifference, decimal QuantityBefore, decimal QuantityAfter,
    string? Reason, bool IsApproved, DateTime CreatedAt);

public record GetStockAdjustmentsQuery(
    Guid CompanyId,
    Guid? WarehouseId = null,
    Guid? ProductId = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<StockAdjustmentDto>>>;
