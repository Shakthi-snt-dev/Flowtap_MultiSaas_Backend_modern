using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateWarehouse;

public class UpdateWarehouseCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateWarehouseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateWarehouseCommand request, CancellationToken ct)
    {
        var warehouse = await db.Warehouses
            .FirstOrDefaultAsync(w => w.CompanyId == request.CompanyId && w.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.Warehouse), request.Id);

        if (request.Name is not null) warehouse.Name = request.Name;
        if (request.Code is not null) warehouse.Code = request.Code;
        if (request.City is not null) warehouse.City = request.City;
        if (request.State is not null) warehouse.State = request.State;
        if (request.Country is not null) warehouse.Country = request.Country;
        if (request.Address is not null) warehouse.Address = request.Address;
        if (request.IsActive.HasValue) warehouse.IsActive = request.IsActive.Value;
        if (request.HasRackSystem.HasValue) warehouse.HasRackSystem = request.HasRackSystem.Value;
        if (request.Type.HasValue &&
            Enum.IsDefined(typeof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums.WarehouseType), request.Type.Value))
            warehouse.Type = (Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums.WarehouseType)request.Type.Value;
        if (request.ManagerEmployeeId.HasValue) warehouse.ManagerEmployeeId = request.ManagerEmployeeId == Guid.Empty
            ? null
            : request.ManagerEmployeeId;

        // Only update LocationId when StoreId is explicitly provided.
        // Leaving StoreId null in the request preserves the existing store link —
        // prevents accidentally clearing it when only updating the manager or other fields.
        if (request.StoreId.HasValue)
            warehouse.LocationId = request.StoreId == Guid.Empty ? null : request.StoreId;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
