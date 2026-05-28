using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Commands.DeleteDeviceModel;

public class DeleteDeviceModelCommandHandler(Flowtap_Repair.DbContext.IRepairDbContext db)
    : IRequestHandler<DeleteDeviceModelCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteDeviceModelCommand request, CancellationToken ct)
    {
        var model = await db.DeviceModels
            .FirstOrDefaultAsync(m => m.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Repair.Domain.Entities.DeviceModel), request.Id);

        model.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

