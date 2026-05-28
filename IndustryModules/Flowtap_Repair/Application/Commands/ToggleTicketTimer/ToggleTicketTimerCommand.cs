using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Commands.ToggleTicketTimer;

public record ToggleTicketTimerCommand(Guid CompanyId, Guid EmployeeId, Guid TicketId) : IRequest<Result<bool>>;

