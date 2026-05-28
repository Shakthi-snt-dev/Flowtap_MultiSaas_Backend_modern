using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteWarehouseRack;

public class DeleteWarehouseRackCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteWarehouseRackCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteWarehouseRackCommand request, CancellationToken ct)
    {
        var rack = await db.WarehouseRacks
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == request.CompanyId, ct);
        if (rack is null)
            return Result<bool>.Failure("Rack not found.");

        rack.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
