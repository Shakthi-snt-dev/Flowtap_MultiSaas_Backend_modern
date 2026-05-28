using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetStockBatches;

public record StockBatchDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    Guid WarehouseId,
    string WarehouseName,
    string? BatchNumber,
    int Quantity,
    decimal CostPrice,
    DateTime ReceivedAt,
    DateTime? ExpiryDate);

public record GetStockBatchesQuery(
    Guid CompanyId,
    Guid? ProductId = null,
    Guid? WarehouseId = null) : IRequest<Result<List<StockBatchDto>>>;
