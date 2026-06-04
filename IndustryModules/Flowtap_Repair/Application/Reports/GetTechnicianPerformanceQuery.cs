using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.Reports.DTOs;
using MediatR;

namespace Flowtap_Repair.Application.Reports;

public record GetTechnicianPerformanceQuery(Guid CompanyId, Guid? LocationId, DateTime From, DateTime To)
    : IRequest<Result<TechnicianPerformanceDto>>;
