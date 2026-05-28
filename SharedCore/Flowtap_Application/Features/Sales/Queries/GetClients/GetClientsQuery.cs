using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Queries.GetClients;

public record GetClientsQuery(
    Guid CompanyId, int Page = 1, int PageSize = 20, string? Search = null,
    Guid? LocationId = null)
    : IRequest<Result<PaginatedList<ClientListItemDto>>>;
