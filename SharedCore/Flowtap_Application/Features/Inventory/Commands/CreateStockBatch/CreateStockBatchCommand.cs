using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateStockBatch;

public record CreateStockBatchCommand(
    Guid CompanyId,
    Guid ProductId,
    Guid WarehouseId,
    string? BatchNumber,
    int Quantity,
    decimal CostPrice,
    DateTime ReceivedAt,
    DateTime? ExpiryDate = null) : IRequest<Result<Guid>>;
