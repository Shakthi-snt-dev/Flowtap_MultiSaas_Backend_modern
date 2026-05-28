using Flowtap_Application.Common.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Food.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.RawMaterials.ConsumeForProduction;

public class ConsumeForProductionCommandHandler(IFoodDbContext db)
    : IRequestHandler<ConsumeForProductionCommand, Result>
{
    public async Task<Result> Handle(ConsumeForProductionCommand request, CancellationToken ct)
    {
        var recipe = await db.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.CompanyId == request.CompanyId
                                   && r.ProductId == request.MenuItemId
                                   && r.IsActive, ct);

        if (recipe is null)
            return Result.Failure("No active recipe found for this menu item.");

        foreach (var ingredient in recipe.Ingredients)
        {
            var deductQty = ingredient.Quantity * request.QuantityProduced;

            var stock = await db.WarehouseStocks
                .FirstOrDefaultAsync(s => s.ProductId == ingredient.RawMaterialProductId
                                       && s.WarehouseId == request.WarehouseId, ct);

            if (stock is null)
                return Result.Failure($"No stock record found for raw material '{ingredient.RawMaterialName}' in this warehouse.");

            if (stock.Quantity < deductQty)
                return Result.Failure($"Insufficient stock for '{ingredient.RawMaterialName}'. Required: {deductQty}, Available: {stock.Quantity}.");

            var quantityBefore = stock.Quantity;
            stock.Quantity          -= deductQty;
            stock.LastMovementAt    = DateTime.UtcNow;

            db.InventoryTransactions.Add(new InventoryTransaction
            {
                CompanyId         = request.CompanyId,
                ProductId         = ingredient.RawMaterialProductId,
                WarehouseId       = request.WarehouseId,
                Type              = InventoryTransactionType.Consumed,
                Quantity          = -deductQty,
                QuantityBefore    = quantityBefore,
                QuantityAfter     = stock.Quantity,
                CostPrice         = stock.Quantity > 0 ? stock.Quantity : 0,
                Reference         = request.KitchenOrderId.HasValue
                                    ? $"KOT:{request.KitchenOrderId.Value}"
                                    : "Production",
                RelatedEntityId   = request.KitchenOrderId,
                TransactionDate   = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
