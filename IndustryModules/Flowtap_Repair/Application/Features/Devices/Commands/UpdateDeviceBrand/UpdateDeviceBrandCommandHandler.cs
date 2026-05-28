using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Commands.UpdateDeviceBrand;

public class UpdateDeviceBrandCommandHandler(Flowtap_Repair.DbContext.IRepairDbContext db)
    : IRequestHandler<UpdateDeviceBrandCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateDeviceBrandCommand request, CancellationToken ct)
    {
        var brand = await db.DeviceBrands
            .FirstOrDefaultAsync(b => b.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Repair.Domain.Entities.DeviceBrand), request.Id);

        if (request.Name is not null)    brand.Name    = request.Name;
        if (request.IconUrl is not null) brand.IconUrl = request.IconUrl;
        if (request.Color is not null)   brand.Color   = request.Color;
        if (request.IsActive.HasValue)   brand.IsActive = request.IsActive.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

