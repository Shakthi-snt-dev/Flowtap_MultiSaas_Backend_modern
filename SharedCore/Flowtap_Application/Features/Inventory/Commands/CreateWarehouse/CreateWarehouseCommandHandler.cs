using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.CreateWarehouse;

public class CreateWarehouseCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateWarehouseCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateWarehouseCommand request, CancellationToken ct)
    {
        var exists = await db.Warehouses
            .AnyAsync(w => w.CompanyId == request.CompanyId && w.Code == request.Code, ct);
        if (exists)
            return Result<Guid>.Failure("Warehouse code already exists.");

        if (!Enum.IsDefined(typeof(WarehouseType), request.Type))
            return Result<Guid>.Failure($"Invalid warehouse type: {request.Type}");

        var warehouseType = (WarehouseType)request.Type;

        if (warehouseType == WarehouseType.InStore)
        {
            var alreadyHasInStore = await db.Warehouses
                .AnyAsync(w => w.CompanyId == request.CompanyId 
                               && w.Type == WarehouseType.InStore 
                               && w.LocationId == request.StoreId, ct);
            if (alreadyHasInStore)
                return Result<Guid>.Failure("This store already has an In-Store warehouse. Only one In-Store warehouse is allowed per store.");
        }

        var warehouse = new Warehouse
        {
            CompanyId = request.CompanyId,
            LocationId = request.StoreId,
            Code = request.Code,
            Name = request.Name,
            Type = warehouseType,
            City = request.City ?? string.Empty,
            State = request.State ?? string.Empty,
            Country = request.Country ?? string.Empty,
            Address = request.Address,
            Status = WarehouseStatus.Active,
            IsActive = true,
            HasRackSystem = request.HasRackSystem
        };

        db.Warehouses.Add(warehouse);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(warehouse.Id);
    }
}
