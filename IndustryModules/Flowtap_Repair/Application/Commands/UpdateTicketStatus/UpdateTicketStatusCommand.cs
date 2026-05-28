using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.UpdateTicketStatus;

public record UpdateTicketStatusCommand(Guid CompanyId, Guid TicketId, string NewStatus, string? Notes)
    : IRequest<Result<bool>>;
