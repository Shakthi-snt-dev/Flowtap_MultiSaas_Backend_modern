using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.TransferStock;

public class TransferStockCommandHandler(IApplicationDbContext db)
    : IRequestHandler<TransferStockCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(TransferStockCommand request, CancellationToken ct)
    {
        var transfer = new InventoryTransfer
        {
            CompanyId = request.CompanyId,
            FromWarehouseId = request.FromWarehouseId,
            ToWarehouseId = request.ToWarehouseId,
            RequestedByEmployeeId = request.RequestedByEmployeeId,
            TransferNumber = $"TRF-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Status = TransferStatus.Draft,
            ScheduledDate = request.ScheduledDate,
            Notes = request.Notes,
            Items = request.Items.Select(i => new InventoryTransferItem
            {
                CompanyId = request.CompanyId,
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        db.InventoryTransfers.Add(transfer);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(transfer.Id);
    }
}
