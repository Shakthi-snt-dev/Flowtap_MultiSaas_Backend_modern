using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Purchase.Commands.UpdatePurchaseOrderStatus;

public record UpdatePurchaseOrderStatusCommand(
    Guid OrderId,
    Guid CompanyId,
    string Status) : IRequest<Result<Guid>>;
