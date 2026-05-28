using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Purchase.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Purchase.Queries.GetPurchaseOrders;

public record GetPurchaseOrdersQuery(Guid CompanyId, string? Status, int Page = 1, int PageSize = 20)
    : IRequest<Result<PaginatedList<PurchaseOrderDto>>>;
