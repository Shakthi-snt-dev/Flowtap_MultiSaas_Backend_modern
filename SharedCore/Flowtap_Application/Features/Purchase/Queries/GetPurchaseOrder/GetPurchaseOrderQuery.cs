using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Purchase.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Purchase.Queries.GetPurchaseOrder;

public record GetPurchaseOrderQuery(Guid CompanyId, Guid OrderId) : IRequest<Result<PurchaseOrderDto>>;
