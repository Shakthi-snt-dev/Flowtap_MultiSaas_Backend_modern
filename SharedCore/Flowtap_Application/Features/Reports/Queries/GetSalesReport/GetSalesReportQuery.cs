using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Reports.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Reports.Queries.GetSalesReport;

public record GetSalesReportQuery(Guid CompanyId, Guid? LocationId, DateTime From, DateTime To)
    : IRequest<Result<SalesReportDto>>;
