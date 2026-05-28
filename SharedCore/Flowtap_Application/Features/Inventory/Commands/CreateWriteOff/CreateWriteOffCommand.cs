using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.CreateWriteOff;

public record CreateWriteOffCommand(
    Guid CompanyId, Guid WarehouseId, Guid ProductId,
    decimal Quantity, string Type, string Reason,
    Guid RequestedByEmployeeId, string? Notes = null) : IRequest<Result<Guid>>;
