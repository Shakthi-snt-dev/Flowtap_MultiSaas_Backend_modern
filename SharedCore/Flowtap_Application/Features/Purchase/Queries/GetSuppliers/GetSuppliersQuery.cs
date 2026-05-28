using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Purchase.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Purchase.Queries.GetSuppliers;

public record GetSuppliersQuery(Guid CompanyId, string? Search = null, bool ActiveOnly = true, int Page = 1, int PageSize = 20)
    : IRequest<Result<PaginatedList<SupplierDto>>>;
