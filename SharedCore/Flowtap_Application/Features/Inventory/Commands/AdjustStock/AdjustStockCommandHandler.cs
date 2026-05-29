using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Common.Notifications;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.AdjustStock;

public class AdjustStockCommandHandler(IApplicationDbContext db, IDateTimeService dateTime, IPublisher publisher)
    : IRequestHandler<AdjustStockCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AdjustStockCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<StockAdjustmentType>(request.AdjustmentType, true, out var adjType))
            return Result<Guid>.Failure($"Invalid adjustment type: {request.AdjustmentType}");

        if (!Enum.TryParse<StockAdjustmentReason>(request.ReasonCode, true, out var reasonCode))
            return Result<Guid>.Failure($"Invalid reason code: {request.ReasonCode}");

        var stock = await db.WarehouseStocks
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId
                && s.WarehouseId == request.WarehouseId
                && s.ProductId == request.ProductId, ct);

        var before = stock?.Quantity ?? 0;
        var after = adjType == StockAdjustmentType.Add ? before + request.Quantity : before - request.Quantity;

        if (after < 0) return Result<Guid>.Failure("Stock cannot go below zero.");

        if (stock is null)
        {
            stock = new WarehouseStock
            {
                CompanyId = request.CompanyId,
                WarehouseId = request.WarehouseId,
                ProductId = request.ProductId,
                Quantity = after,
                LastMovementAt = dateTime.UtcNow
            };
            db.WarehouseStocks.Add(stock);
        }
        else
        {
            stock.Quantity = after;
            stock.LastMovementAt = dateTime.UtcNow;
        }

        var count = await db.StockAdjustments.CountAsync(s => s.CompanyId == request.CompanyId, ct);
        var adjustment = new StockAdjustment
        {
            CompanyId = request.CompanyId,
            WarehouseId = request.WarehouseId,
            ProductId = request.ProductId,
            AdjustmentNumber = $"ADJ-{count + 1:D6}",
            AdjustmentType = adjType,
            QuantityDifference = request.Quantity,
            ReasonCode = reasonCode,
            Reason = request.Reason,
            Notes = request.Notes,
            QuantityBefore = before,
            QuantityAfter = after,
            AdjustedByEmployeeId = request.AdjustedByEmployeeId,
            IsApproved = true
        };

        db.StockAdjustments.Add(adjustment);

        db.InventoryTransactions.Add(new InventoryTransaction
        {
            CompanyId = request.CompanyId,
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            Type = InventoryTransactionType.Adjustment,
            Quantity = adjType == StockAdjustmentType.Add ? request.Quantity : -request.Quantity,
            QuantityBefore = before,
            QuantityAfter = after,
            CostPrice = 0,
            Reference = adjustment.AdjustmentNumber
        });

        await db.SaveChangesAsync(ct);

        // Auto-check stock alert rules whenever stock is REDUCED.
        // FoodStockCheckOnSaleHandler and ReorderCheckOnSaleHandler listen to this
        // notification and will queue alert messages if any rule's threshold is crossed.
        if (adjType == StockAdjustmentType.Remove)
        {
            await publisher.Publish(new SaleStockDeductedNotification(
                CompanyId:   request.CompanyId,
                WarehouseId: request.WarehouseId,
                Items:       [new DeductedStockItem(request.ProductId)]), ct);
        }

        return Result<Guid>.Success(adjustment.Id);
    }
}
