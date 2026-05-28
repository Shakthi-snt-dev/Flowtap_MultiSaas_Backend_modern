using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetReorderAlerts;

public record GetReorderAlertsQuery(Guid CompanyId, bool UnhandledOnly = true)
    : IRequest<Result<List<ReorderAlertDto>>>;
