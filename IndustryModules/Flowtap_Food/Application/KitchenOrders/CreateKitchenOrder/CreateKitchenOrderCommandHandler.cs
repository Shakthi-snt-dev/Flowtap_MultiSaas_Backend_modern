using Flowtap_Application.Common.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Entities;
using Flowtap_Food.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.KitchenOrders.CreateKitchenOrder;

public class CreateKitchenOrderCommandHandler(IFoodDbContext db)
    : IRequestHandler<CreateKitchenOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateKitchenOrderCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<FoodOrderType>(request.OrderType, true, out var orderType))
            orderType = FoodOrderType.DineIn;

        var count = await db.KitchenOrders.CountAsync(k => k.CompanyId == request.CompanyId, ct);

        var kot = new KitchenOrder
        {
            CompanyId  = request.CompanyId,
            LocationId = request.LocationId,
            SaleId     = request.SaleId,
            TableId    = request.TableId,
            OrderType  = orderType,
            Status     = KOTStatus.New,
            KotNumber  = $"KOT-{count + 1:D5}",
            Notes      = request.Notes,
            Items      = request.Items.Select(i => new KitchenOrderItem
            {
                ProductId   = i.ProductId,
                ProductName = i.ProductName,
                Quantity    = i.Quantity,
                Notes       = i.Notes
            }).ToList()
        };

        // Mark table as Occupied if DineIn
        if (request.TableId.HasValue && orderType == FoodOrderType.DineIn)
        {
            var table = await db.FoodTables.FindAsync([request.TableId.Value], ct);
            if (table is not null)
            {
                table.Status        = FoodTableStatus.Occupied;
                table.CurrentSaleId = request.SaleId;
            }
        }

        db.KitchenOrders.Add(kot);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(kot.Id);
    }
}
