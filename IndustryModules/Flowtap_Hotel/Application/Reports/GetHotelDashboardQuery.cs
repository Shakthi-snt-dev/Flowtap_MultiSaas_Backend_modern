using Flowtap_Application.Common.DTOs;
using Flowtap_Hotel.Application.Reports.DTOs;
using MediatR;

namespace Flowtap_Hotel.Application.Reports;

public record GetHotelDashboardQuery(Guid CompanyId, Guid? LocationId)
    : IRequest<Result<HotelDashboardDto>>;
