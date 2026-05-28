using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Commands.RemoveProductDeviceMapping;

public class RemoveProductDeviceMappingCommandHandler(Flowtap_Repair.DbContext.IRepairDbContext db)
    : IRequestHandler<RemoveProductDeviceMappingCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveProductDeviceMappingCommand request, CancellationToken ct)
    {
        // verify ownership via a join, then find mapping
        var productOwned = await db.Products
            .AnyAsync(p => p.Id == request.ProductId && p.CompanyId == request.CompanyId, ct);
        if (!productOwned)
            return Result<bool>.Failure("Product not found.");

        var mapping = await db.ProductDeviceModelMappings
            .FirstOrDefaultAsync(m =>
                m.ProductId == request.ProductId &&
                m.DeviceModelId == request.DeviceModelId, ct);

        if (mapping is null)
            return Result<bool>.Failure("Mapping not found.");

        db.ProductDeviceModelMappings.Remove(mapping);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

