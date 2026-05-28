using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Queries.GetTickets;

public record GetTicketsQuery(Guid CompanyId, Guid? LocationId, string? Status, int Page = 1, int PageSize = 20)
    : IRequest<Result<PaginatedList<TicketListDto>>>;

