using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.CreateWarehouseRack;

public class CreateWarehouseRackCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateWarehouseRackCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateWarehouseRackCommand request, CancellationToken ct)
    {
        var exists = await db.WarehouseRacks
            .AnyAsync(r => r.CompanyId == request.CompanyId && r.WarehouseId == request.WarehouseId && r.Code == request.Code, ct);
        if (exists)
            return Result<Guid>.Failure("Rack code already exists in this warehouse.");

        ZoneType? zoneType = null;
        if (request.ZoneType is not null && Enum.TryParse<ZoneType>(request.ZoneType, true, out var parsedZone))
            zoneType = parsedZone;

        RackType? rackType = null;
        if (request.Type is not null && Enum.TryParse<RackType>(request.Type, true, out var parsedRack))
            rackType = parsedRack;

        var rack = new WarehouseRack
        {
            CompanyId = request.CompanyId,
            WarehouseId = request.WarehouseId,
            Code = request.Code,
            Name = request.Name,
            ZoneLabel = request.ZoneLabel,
            ZoneType = zoneType,
            Type = rackType,
            MaxLoadKg = request.MaxLoadKg,
            Levels = request.Levels,
            IsActive = true
        };

        db.WarehouseRacks.Add(rack);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(rack.Id);
    }
}
