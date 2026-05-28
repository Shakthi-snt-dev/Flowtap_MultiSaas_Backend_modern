using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.CreateWriteOff;

public class CreateWriteOffCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateWriteOffCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateWriteOffCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<InventoryWriteOffType>(request.Type, true, out var writeOffType))
            return Result<Guid>.Failure($"Invalid write-off type: {request.Type}");

        var stock = await db.WarehouseStocks
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId
                && s.WarehouseId == request.WarehouseId
                && s.ProductId == request.ProductId, ct);

        if (stock is null || stock.Quantity < request.Quantity)
            return Result<Guid>.Failure("Insufficient stock for write-off.");

        var count = await db.InventoryWriteOffs.CountAsync(w => w.CompanyId == request.CompanyId, ct);
        var writeOff = new InventoryWriteOff
        {
            CompanyId = request.CompanyId,
            WarehouseId = request.WarehouseId,
            ProductId = request.ProductId,
            WriteOffNumber = $"WO-{count + 1:D6}",
            Quantity = request.Quantity,
            UnitCost = 0, // Will be computed from cost layers in real impl
            Type = writeOffType,
            Reason = request.Reason,
            Notes = request.Notes,
            RequestedByEmployeeId = request.RequestedByEmployeeId,
            RequiresApproval = true,
            Status = WriteOffStatus.Pending
        };

        db.InventoryWriteOffs.Add(writeOff);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(writeOff.Id);
    }
}
