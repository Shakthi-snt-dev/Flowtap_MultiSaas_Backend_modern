using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateInventorySerial;

public class UpdateInventorySerialCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateInventorySerialCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateInventorySerialCommand request, CancellationToken ct)
    {
        var serial = await db.InventorySerials
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.CompanyId == request.CompanyId, ct);
        if (serial is null)
            return Result<bool>.Failure("Serial not found.");

        if (request.IsSold.HasValue)            serial.IsSold = request.IsSold.Value;
        if (request.IsReturned.HasValue)        serial.IsReturned = request.IsReturned.Value;
        if (request.IsActive.HasValue)          serial.IsActive = request.IsActive.Value;
        if (request.WarrantyEndDate.HasValue)   serial.WarrantyEndDate = request.WarrantyEndDate;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
