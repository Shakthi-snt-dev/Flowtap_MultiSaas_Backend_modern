using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateWarehouseRack;

public class UpdateWarehouseRackCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateWarehouseRackCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateWarehouseRackCommand request, CancellationToken ct)
    {
        var rack = await db.WarehouseRacks
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == request.CompanyId, ct);
        if (rack is null)
            return Result<bool>.Failure("Rack not found.");

        if (request.Name is not null)     rack.Name = request.Name;
        if (request.ZoneLabel is not null) rack.ZoneLabel = request.ZoneLabel;
        if (request.MaxLoadKg.HasValue)   rack.MaxLoadKg = request.MaxLoadKg;
        if (request.Levels.HasValue)      rack.Levels = request.Levels;
        if (request.IsActive.HasValue)    rack.IsActive = request.IsActive.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
