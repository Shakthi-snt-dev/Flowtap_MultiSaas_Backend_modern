using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetWarehouseBins;

public class GetWarehouseBinsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetWarehouseBinsQuery, Result<List<WarehouseBinDto>>>
{
    public async Task<Result<List<WarehouseBinDto>>> Handle(GetWarehouseBinsQuery request, CancellationToken ct)
    {
        var items = await db.WarehouseBins
            .Where(b => b.RackId == request.RackId && b.CompanyId == request.CompanyId)
            .OrderBy(b => b.Level).ThenBy(b => b.Position).ThenBy(b => b.Code)
            .Select(b => new WarehouseBinDto(
                b.Id, b.Code, b.Level, b.Position, b.MaxWeightKg,
                b.Status.ToString(), b.IsActive))
            .ToListAsync(ct);

        return Result<List<WarehouseBinDto>>.Success(items);
    }
}
