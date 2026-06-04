using Flowtap_Application.Common.DTOs;
using MediatR;
using Flowtap_Food.Application.Reports.DTOs;

namespace Flowtap_Food.Application.Reports;

public record GetFoodDashboardQuery(Guid CompanyId, Guid? LocationId)
    : IRequest<Result<FoodDashboardDto>>;
