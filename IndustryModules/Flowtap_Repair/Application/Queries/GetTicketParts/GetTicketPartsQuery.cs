using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.Commands.AddPartToTicket;
using MediatR;

namespace Flowtap_Repair.Application.Queries.GetTicketParts;

public record GetTicketPartsQuery(
    Guid CompanyId,
    Guid ServiceTicketId) : IRequest<Result<List<TicketPartUsageDto>>>;

