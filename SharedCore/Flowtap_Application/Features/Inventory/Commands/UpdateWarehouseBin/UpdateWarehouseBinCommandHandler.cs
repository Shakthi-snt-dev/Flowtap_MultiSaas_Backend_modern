using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateWarehouseBin;

public class UpdateWarehouseBinCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateWarehouseBinCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateWarehouseBinCommand request, CancellationToken ct)
    {
        var bin = await db.WarehouseBins
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == request.CompanyId, ct);
        if (bin is null)
            return Result<bool>.Failure("Bin not found.");

        if (request.Code is not null)       bin.Code = request.Code;
        if (request.Level.HasValue)         bin.Level = request.Level;
        if (request.Position.HasValue)      bin.Position = request.Position;
        if (request.MaxWeightKg.HasValue)   bin.MaxWeightKg = request.MaxWeightKg;
        if (request.IsActive.HasValue)      bin.IsActive = request.IsActive.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
