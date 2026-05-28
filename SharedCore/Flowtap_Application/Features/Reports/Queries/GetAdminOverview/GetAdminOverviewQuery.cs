using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Reports.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Reports.Queries.GetAdminOverview;

public record GetAdminOverviewQuery(Guid CompanyId) : IRequest<Result<AdminOverviewDto>>;
