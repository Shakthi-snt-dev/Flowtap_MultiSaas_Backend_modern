using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Commands.DeleteDeviceBrand;

public class DeleteDeviceBrandCommandHandler(Flowtap_Repair.DbContext.IRepairDbContext db)
    : IRequestHandler<DeleteDeviceBrandCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteDeviceBrandCommand request, CancellationToken ct)
    {
        var brand = await db.DeviceBrands
            .FirstOrDefaultAsync(b => b.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Repair.Domain.Entities.DeviceBrand), request.Id);

        brand.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

