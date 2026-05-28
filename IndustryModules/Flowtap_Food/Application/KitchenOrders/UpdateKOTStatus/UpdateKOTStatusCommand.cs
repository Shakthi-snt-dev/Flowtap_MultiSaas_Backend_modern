using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.KitchenOrders.UpdateKOTStatus;

public record UpdateKOTStatusCommand(Guid Id, Guid CompanyId, string Status) : IRequest<Result>;
