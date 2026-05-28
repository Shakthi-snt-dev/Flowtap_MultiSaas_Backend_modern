using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Queries.GetClientPurchases;

public record GetClientPurchasesQuery(
    Guid CompanyId,
    Guid ClientId,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<ClientPurchaseDto>>>;
