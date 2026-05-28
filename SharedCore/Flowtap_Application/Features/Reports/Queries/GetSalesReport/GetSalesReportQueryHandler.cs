using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Reports.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Reports.Queries.GetSalesReport;

public class GetSalesReportQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetSalesReportQuery, Result<SalesReportDto>>
{
    public async Task<Result<SalesReportDto>> Handle(GetSalesReportQuery request, CancellationToken ct)
    {
        var query = db.Sales
            .Where(s => s.CompanyId == request.CompanyId
                && s.CreatedAt >= request.From && s.CreatedAt <= request.To
                && s.Status == SaleStatus.Completed);

        if (request.LocationId.HasValue)
            query = query.Where(s => s.LocationId == request.LocationId);

        var sales = await query.ToListAsync(ct);

        var daily = sales
            .GroupBy(s => DateOnly.FromDateTime(s.CreatedAt))
            .Select(g => new SalesSummaryDto(g.Key, g.Sum(s => s.TotalAmount), g.Count()))
            .OrderBy(d => d.Date)
            .ToList();

        return Result<SalesReportDto>.Success(new SalesReportDto(
            request.From, request.To,
            sales.Sum(s => s.TotalAmount),
            sales.Sum(s => s.TaxAmount),
            sales.Count,
            daily));
    }
}
