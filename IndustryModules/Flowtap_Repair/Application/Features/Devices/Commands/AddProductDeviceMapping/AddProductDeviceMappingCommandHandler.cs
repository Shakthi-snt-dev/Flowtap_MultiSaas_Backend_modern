using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Commands.AddProductDeviceMapping;

public class AddProductDeviceMappingCommandHandler(Flowtap_Repair.DbContext.IRepairDbContext db)
    : IRequestHandler<AddProductDeviceMappingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddProductDeviceMappingCommand request, CancellationToken ct)
    {
        // verify product belongs to company
        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.CompanyId == request.CompanyId, ct);
        if (product is null)
            return Result<Guid>.Failure("Product not found.");

        // check mapping does not already exist
        var exists = await db.ProductDeviceModelMappings
            .AnyAsync(m => m.ProductId == request.ProductId && m.DeviceModelId == request.DeviceModelId, ct);
        if (exists)
            return Result<Guid>.Failure("This device model is already linked to the product.");

        var mapping = new ProductDeviceModelMapping
        {
            ProductId     = request.ProductId,
            DeviceModelId = request.DeviceModelId,
        };
        db.ProductDeviceModelMappings.Add(mapping);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(mapping.Id);
    }
}

