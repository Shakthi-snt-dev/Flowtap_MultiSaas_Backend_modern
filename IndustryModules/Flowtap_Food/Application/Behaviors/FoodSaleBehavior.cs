using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Sales.Commands.CreateSale;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Entities;
using Flowtap_Food.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior registered ONLY in Flowtap_Food_API.
/// Runs AFTER core CreateSaleHandler completes — automatically creates a KOT
/// and updates the table status. Never loaded by Repair, Hotel, or Medical APIs.
/// </summary>
public class FoodSaleBehavior(IFoodDbContext db)
    : IPipelineBehavior<CreateSaleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateSaleCommand request,
        RequestHandlerDelegate<Result<Guid>> next,
        CancellationToken ct)
    {
        // Step 1: run the normal sale (core handler — completely unchanged)
        var result = await next();
        if (!result.IsSuccess) return result;

        // Step 2: only proceed if this is a food sale (has TableId or FoodOrderType)
        if (request.TableId is null && request.FoodOrderType is null)
            return result;

        var orderType = request.FoodOrderType ?? FoodOrderType.Takeaway;

        // Step 3: count existing KOTs for KOT number generation
        var count = await db.KitchenOrders.CountAsync(k => k.CompanyId == request.CompanyId, ct);

        var kot = new KitchenOrder
        {
            CompanyId  = request.CompanyId,
            LocationId = request.LocationId,
            SaleId     = result.Value,
            TableId    = request.TableId,
            OrderType  = orderType,
            Status     = KOTStatus.New,
            KotNumber  = $"KOT-{count + 1:D5}",
            Items      = request.Items.Select(i => new KitchenOrderItem
            {
                ProductId   = i.ProductId,
                ProductName = i.ProductName,
                Quantity    = i.Quantity,
                Notes       = i.Notes,           // special instructions ("no onion") → sent to kitchen
            }).ToList()
        };

        db.KitchenOrders.Add(kot);

        // Step 4: mark table as Occupied (DineIn only)
        if (request.TableId.HasValue && orderType == FoodOrderType.DineIn)
        {
            var table = await db.FoodTables.FindAsync([request.TableId.Value], ct);
            if (table is not null)
            {
                table.Status        = FoodTableStatus.Occupied;
                table.CurrentSaleId = result.Value;
            }
        }

        await db.SaveChangesAsync(ct);
        return result;
    }
}
