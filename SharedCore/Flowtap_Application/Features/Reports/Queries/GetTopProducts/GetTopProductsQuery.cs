using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Reports.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Reports.Queries.GetTopProducts;

public record GetTopProductsQuery(Guid CompanyId, Guid? LocationId, DateTime From, DateTime To, int Top = 10)
    : IRequest<Result<TopProductsReportDto>>;
