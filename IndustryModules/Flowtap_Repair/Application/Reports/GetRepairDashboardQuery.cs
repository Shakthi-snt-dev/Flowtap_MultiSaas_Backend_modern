using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.Reports.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Reports;

public record GetRepairDashboardQuery(Guid CompanyId, Guid? LocationId)
    : IRequest<Result<RepairDashboardDto>>;
