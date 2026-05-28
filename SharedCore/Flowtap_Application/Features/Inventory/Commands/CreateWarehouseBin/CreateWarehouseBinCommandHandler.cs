using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.CreateWarehouseBin;

public class CreateWarehouseBinCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateWarehouseBinCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateWarehouseBinCommand request, CancellationToken ct)
    {
        var rackExists = await db.WarehouseRacks
            .AnyAsync(r => r.Id == request.RackId && r.CompanyId == request.CompanyId, ct);
        if (!rackExists)
            return Result<Guid>.Failure("Rack not found.");

        var exists = await db.WarehouseBins
            .AnyAsync(b => b.RackId == request.RackId && b.Code == request.Code, ct);
        if (exists)
            return Result<Guid>.Failure("Bin code already exists in this rack.");

        var bin = new WarehouseBin
        {
            CompanyId = request.CompanyId,
            RackId = request.RackId,
            Code = request.Code,
            Level = request.Level,
            Position = request.Position,
            MaxWeightKg = request.MaxWeightKg,
            Status = BinStatus.Empty,
            IsActive = true
        };

        db.WarehouseBins.Add(bin);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(bin.Id);
    }
}
