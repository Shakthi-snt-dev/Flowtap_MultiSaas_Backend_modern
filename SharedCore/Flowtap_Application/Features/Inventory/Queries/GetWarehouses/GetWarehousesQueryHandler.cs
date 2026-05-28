using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetWarehouses;

public class GetWarehousesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetWarehousesQuery, Result<List<WarehouseDto>>>
{
    public async Task<Result<List<WarehouseDto>>> Handle(GetWarehousesQuery request, CancellationToken ct)
    {
        var query = db.Warehouses.Where(w => w.CompanyId == request.CompanyId);
        if (request.ActiveOnly) query = query.Where(w => w.IsActive);

        var storeId = currentUser.StoreId;
        if (storeId.HasValue && storeId.Value != Guid.Empty)
        {
            query = query.Where(w => w.LocationId == storeId.Value || w.LocationId == null);
        }

        var items = await query.OrderBy(w => w.Name).ToListAsync(ct);

        var dtos = items.Select(w => new WarehouseDto(
            w.Id, w.CompanyId, w.Code, w.Name, w.Type.ToString(),
            w.Status.ToString(), w.City, w.Country, w.IsActive, w.HasRackSystem)).ToList();

        return Result<List<WarehouseDto>>.Success(dtos);
    }
}
