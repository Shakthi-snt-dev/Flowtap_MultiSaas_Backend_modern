using Flowtap_Application.Common.DTOs;
using Flowtap_Medical.Application.Reports.DTOs;
using MediatR;

namespace Flowtap_Medical.Application.Reports;

public record GetMedicalDashboardQuery(Guid CompanyId, Guid? LocationId)
    : IRequest<Result<MedicalDashboardDto>>;
