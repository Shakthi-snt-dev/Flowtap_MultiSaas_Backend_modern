using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetWarehouseRacks;

public class GetWarehouseRacksQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetWarehouseRacksQuery, Result<List<WarehouseRackDto>>>
{
    public async Task<Result<List<WarehouseRackDto>>> Handle(GetWarehouseRacksQuery request, CancellationToken ct)
    {
        var items = await db.WarehouseRacks
            .Where(r => r.CompanyId == request.CompanyId && r.WarehouseId == request.WarehouseId)
            .OrderBy(r => r.Code)
            .Select(r => new WarehouseRackDto(
                r.Id, r.Code, r.Name, r.ZoneLabel,
                r.ZoneType.HasValue ? r.ZoneType.Value.ToString() : null,
                r.Type.HasValue ? r.Type.Value.ToString() : null,
                r.MaxLoadKg, r.Levels, r.IsActive))
            .ToListAsync(ct);

        return Result<List<WarehouseRackDto>>.Success(items);
    }
}
