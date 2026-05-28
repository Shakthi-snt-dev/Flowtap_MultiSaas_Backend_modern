using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.AddPartToTicket;

public record TicketPartUsageDto(
    Guid Id,
    Guid ServiceTicketId,
    Guid ProductId,
    Guid WarehouseId,
    decimal Quantity,
    decimal UnitPrice,
    DateTime UsedAt);

public record AddPartToTicketCommand(
    Guid CompanyId,
    Guid ServiceTicketId,
    Guid ProductId,
    Guid WarehouseId,
    decimal Quantity,
    decimal UnitPrice) : IRequest<Result<TicketPartUsageDto>>;

