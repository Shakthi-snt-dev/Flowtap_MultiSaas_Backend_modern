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

        // Store filter priority:
        // 1. Explicit storeId from query param (frontend passes currentStoreId when switching stores)
        // 2. Fall back to JWT storeId (for employees whose store is fixed in token)
        // 3. If neither — owner with no store context — show all warehouses
        var storeId = (request.StoreId.HasValue && request.StoreId.Value != Guid.Empty)
            ? request.StoreId
            : currentUser.StoreId;

        if (storeId.HasValue && storeId.Value != Guid.Empty)
        {
            query = query.Where(w => w.LocationId == storeId.Value);
        }

        var items = await query.OrderBy(w => w.Name).ToListAsync(ct);

        var dtos = items.Select(w => new WarehouseDto(
            w.Id, w.CompanyId, w.Code, w.Name, w.Type.ToString(),
            w.Status.ToString(), w.City, w.Country, w.IsActive, w.HasRackSystem,
            w.ManagerEmployeeId, w.LocationId)).ToList();

        return Result<List<WarehouseDto>>.Success(dtos);
    }
}
