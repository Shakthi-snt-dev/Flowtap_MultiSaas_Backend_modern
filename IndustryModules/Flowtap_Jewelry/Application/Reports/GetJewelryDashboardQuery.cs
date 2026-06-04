using Flowtap_Application.Common.DTOs;
using Flowtap_Jewelry.Application.Reports.DTOs;
using MediatR;

namespace Flowtap_Jewelry.Application.Reports;

public record GetJewelryDashboardQuery(Guid CompanyId, Guid? LocationId)
    : IRequest<Result<JewelryDashboardDto>>;
