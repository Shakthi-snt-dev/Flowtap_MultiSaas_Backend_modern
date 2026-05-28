using Flowtap_Application.Common.DTOs;
using Flowtap_Food.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.Tables.GetTables;

public class GetTablesQueryHandler(IFoodDbContext db)
    : IRequestHandler<GetTablesQuery, Result<List<FoodTableDto>>>
{
    public async Task<Result<List<FoodTableDto>>> Handle(GetTablesQuery request, CancellationToken ct)
    {
        var query = db.FoodTables
            .Where(t => t.CompanyId == request.CompanyId && t.IsActive);

        if (request.LocationId.HasValue)
            query = query.Where(t => t.LocationId == request.LocationId.Value);

        var tables = await query
            .OrderBy(t => t.Section)
            .ThenBy(t => t.Name)
            .Select(t => new FoodTableDto(
                t.Id,
                t.LocationId,
                t.Name,
                t.Capacity,
                t.Section,
                t.Status.ToString(),
                t.CurrentSaleId,
                t.IsActive))
            .ToListAsync(ct);

        return Result<List<FoodTableDto>>.Success(tables);
    }
}
