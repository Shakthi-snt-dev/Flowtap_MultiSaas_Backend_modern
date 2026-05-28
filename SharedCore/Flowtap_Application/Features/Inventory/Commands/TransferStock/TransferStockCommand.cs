using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.TransferStock;

public record TransferStockCommand(
    Guid CompanyId, Guid FromWarehouseId, Guid ToWarehouseId,
    Guid RequestedByEmployeeId, List<TransferStockItemDto> Items,
    DateTime? ScheduledDate = null, string? Notes = null) : IRequest<Result<Guid>>;

public record TransferStockItemDto(Guid ProductId, decimal Quantity);
