using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Commands.UpdateDeviceModel;

public class UpdateDeviceModelCommandHandler(Flowtap_Repair.DbContext.IRepairDbContext db)
    : IRequestHandler<UpdateDeviceModelCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateDeviceModelCommand request, CancellationToken ct)
    {
        var model = await db.DeviceModels
            .FirstOrDefaultAsync(m => m.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Repair.Domain.Entities.DeviceModel), request.Id);

        if (request.Name is not null)     model.Name     = request.Name;
        if (request.ImageUrl is not null) model.ImageUrl = request.ImageUrl;
        if (request.IsActive.HasValue)    model.IsActive  = request.IsActive.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

