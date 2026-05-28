using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.CreateInventorySerial;

public class CreateInventorySerialCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateInventorySerialCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateInventorySerialCommand request, CancellationToken ct)
    {
        if (request.ManufacturerSerial is not null)
        {
            var duplicate = await db.InventorySerials
                .AnyAsync(s => s.CompanyId == request.CompanyId && s.ManufacturerSerial == request.ManufacturerSerial, ct);
            if (duplicate)
                return Result<Guid>.Failure("A serial with this manufacturer serial already exists.");
        }

        var serial = new InventorySerial
        {
            CompanyId = request.CompanyId,
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            ManufacturerSerial = request.ManufacturerSerial,
            CompanySerial = request.CompanySerial ?? string.Empty,
            DisplayName = request.DisplayName ?? string.Empty,
            WarrantyStartDate = request.WarrantyStartDate,
            WarrantyEndDate = request.WarrantyEndDate,
            IsActive = true
        };

        db.InventorySerials.Add(serial);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(serial.Id);
    }
}
