using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Queries.GetSales;

public record GetSalesQuery(Guid CompanyId, Guid? LocationId, int Page = 1, int PageSize = 20, string? Status = null)
    : IRequest<Result<PaginatedList<SaleDto>>>;
