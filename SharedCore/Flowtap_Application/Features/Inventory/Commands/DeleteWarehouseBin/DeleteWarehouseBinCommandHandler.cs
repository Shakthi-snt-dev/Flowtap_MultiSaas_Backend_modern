using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteWarehouseBin;

public class DeleteWarehouseBinCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteWarehouseBinCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteWarehouseBinCommand request, CancellationToken ct)
    {
        var bin = await db.WarehouseBins
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == request.CompanyId, ct);
        if (bin is null)
            return Result<bool>.Failure("Bin not found.");

        bin.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
