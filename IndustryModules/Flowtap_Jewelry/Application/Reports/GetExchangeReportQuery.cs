using Flowtap_Application.Common.DTOs;
using Flowtap_Jewelry.Application.Reports.DTOs;
using MediatR;

namespace Flowtap_Jewelry.Application.Reports;

public record GetExchangeReportQuery(Guid CompanyId, Guid? LocationId, DateTime From, DateTime To)
    : IRequest<Result<ExchangeReportDto>>;
