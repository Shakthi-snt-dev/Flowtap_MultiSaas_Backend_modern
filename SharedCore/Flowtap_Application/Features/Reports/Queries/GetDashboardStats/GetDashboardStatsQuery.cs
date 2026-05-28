using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Reports.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Reports.Queries.GetDashboardStats;

public record GetDashboardStatsQuery(Guid CompanyId, Guid? LocationId = null)
    : IRequest<Result<DashboardStatsDto>>;
