using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Purchase.Commands.UpdatePurchaseOrderStatus;

public class UpdatePurchaseOrderStatusCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdatePurchaseOrderStatusCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdatePurchaseOrderStatusCommand request, CancellationToken ct)
    {
        var order = await db.PurchaseOrders
            .FirstOrDefaultAsync(p => p.CompanyId == request.CompanyId && p.Id == request.OrderId, ct)
            ?? throw new NotFoundException(nameof(PurchaseOrder), request.OrderId);

        if (!Enum.TryParse<PurchaseOrderStatus>(request.Status, ignoreCase: true, out var status))
            return Result<Guid>.Failure($"Invalid status: {request.Status}");

        order.Status = status;
        order.StatusUpdatedAt = DateTime.UtcNow;

        if (status == PurchaseOrderStatus.Received)
        {
            var items = await db.PurchaseOrders
                .Include(p => p.Items)
                .Where(p => p.Id == request.OrderId)
                .SelectMany(p => p.Items)
                .ToListAsync(ct);

            foreach (var item in items)
            {
                var stock = await db.WarehouseStocks
                    .FirstOrDefaultAsync(ws =>
                        ws.WarehouseId == order.WarehouseId &&
                        ws.ProductId == item.ProductId &&
                        ws.CompanyId == order.CompanyId, ct);

                if (stock is null)
                {
                    stock = new WarehouseStock
                    {
                        CompanyId = order.CompanyId,
                        WarehouseId = order.WarehouseId,
                        ProductId = item.ProductId,
                    };
                    db.WarehouseStocks.Add(stock);
                }

                var before = stock.Quantity;
                stock.Quantity += item.Quantity;
                stock.LastMovementAt = DateTime.UtcNow;
                stock.LastPurchasedAt = DateTime.UtcNow;

                db.InventoryTransactions.Add(new InventoryTransaction
                {
                    CompanyId = order.CompanyId,
                    ProductId = item.ProductId,
                    WarehouseId = order.WarehouseId,
                    Type = InventoryTransactionType.Purchase,
                    Quantity = item.Quantity,
                    QuantityBefore = before,
                    QuantityAfter = stock.Quantity,
                    CostPrice = item.UnitCost,
                    Reference = order.PONumber,
                    RelatedEntityId = order.Id,
                    TransactionDate = DateTime.UtcNow,
                });
            }
        }

        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(order.Id);
    }
}
