using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Purchase.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreatePurchaseOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseOrderCommand request, CancellationToken ct)
    {
        var count = await db.PurchaseOrders.CountAsync(p => p.CompanyId == request.CompanyId, ct);

        var items = request.Items.Select(i => new PurchaseOrderItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            UnitCost = i.UnitCost,
            TaxPercent = i.TaxPercent
        }).ToList();

        var subTotal = items.Sum(i => i.UnitCost * i.Quantity);
        var taxAmount = items.Sum(i => i.UnitCost * i.Quantity * i.TaxPercent / 100);

        var po = new PurchaseOrder
        {
            CompanyId = request.CompanyId,
            SupplierId = request.SupplierId,
            WarehouseId = request.WarehouseId,
            LocationId = request.LocationId,
            PONumber = $"PO-{count + 1:D6}",
            ExpectedDeliveryDate = request.ExpectedDeliveryDate.HasValue
                ? DateTime.SpecifyKind(request.ExpectedDeliveryDate.Value, DateTimeKind.Utc)
                : null,
            Currency = request.Currency,
            SubTotal = Math.Round(subTotal, 2),
            TaxAmount = Math.Round(taxAmount, 2),
            TotalAmount = Math.Round(subTotal + taxAmount, 2),
            CreatedByEmployeeId = request.CreatedByEmployeeId,
            Items = items
        };

        db.PurchaseOrders.Add(po);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(po.Id);
    }
}
