using Flowtap_Application.Common.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Food.Application.Reports.DTOs;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.Reports;

public class GetTableTurnoverQueryHandler(IFoodDbContext db)
    : IRequestHandler<GetTableTurnoverQuery, Result<TableTurnoverDto>>
{
    public async Task<Result<TableTurnoverDto>> Handle(GetTableTurnoverQuery request, CancellationToken ct)
    {
        var tableQuery = db.FoodTables.Where(t => t.CompanyId == request.CompanyId && t.IsActive);
        if (request.LocationId.HasValue)
            tableQuery = tableQuery.Where(t => t.LocationId == request.LocationId.Value);

        var tables = await tableQuery
            .Select(t => new { t.Id, t.Name, t.Section })
            .ToListAsync(ct);

        var salesQuery = db.Sales
            .Where(s => s.CompanyId == request.CompanyId
                     && s.TableId != null
                     && s.CreatedAt >= request.From
                     && s.CreatedAt < request.To.AddDays(1)
                     && s.FoodOrderType == FoodOrderType.DineIn);

        if (request.LocationId.HasValue)
            salesQuery = salesQuery.Where(s => s.LocationId == request.LocationId.Value);

        var sales = await salesQuery
            .Select(s => new { s.TableId, s.TotalAmount })
            .ToListAsync(ct);

        var salesByTable = sales
            .Where(s => s.TableId.HasValue)
            .GroupBy(s => s.TableId!.Value)
            .ToDictionary(g => g.Key, g => (Count: g.Count(), Revenue: g.Sum(s => s.TotalAmount)));

        var summaries = tables.Select(t =>
        {
            var data = salesByTable.TryGetValue(t.Id, out var d) ? d : (Count: 0, Revenue: 0m);
            return new TableSummaryDto(t.Id, t.Name, t.Section, data.Count, data.Revenue, data.Count);
        }).ToList();

        return Result<TableTurnoverDto>.Success(new TableTurnoverDto(request.From, request.To, summaries));
    }
}
