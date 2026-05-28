using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Queries.GetTicket;

public record GetTicketQuery(Guid CompanyId, Guid TicketId) : IRequest<Result<TicketDto>>;

