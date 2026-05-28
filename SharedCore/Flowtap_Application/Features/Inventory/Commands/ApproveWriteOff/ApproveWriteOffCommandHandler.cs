using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.ApproveWriteOff;

public class ApproveWriteOffCommandHandler(IApplicationDbContext db)
    : IRequestHandler<ApproveWriteOffCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ApproveWriteOffCommand request, CancellationToken ct)
    {
        var writeOff = await db.InventoryWriteOffs
            .FirstOrDefaultAsync(w => w.Id == request.Id && w.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.InventoryWriteOff), request.Id);

        writeOff.Status = request.Approved ? WriteOffStatus.Approved : WriteOffStatus.Rejected;
        writeOff.ApprovedByEmployeeId = request.ApprovedByEmployeeId;
        writeOff.ApprovedAt = DateTime.UtcNow;
        if (request.Notes is not null) writeOff.Notes = request.Notes;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
