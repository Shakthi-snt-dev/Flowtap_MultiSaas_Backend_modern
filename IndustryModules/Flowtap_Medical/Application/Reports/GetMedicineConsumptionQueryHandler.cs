using Flowtap_Application.Common.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Medical.Application.Reports.DTOs;
using Flowtap_Medical.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Medical.Application.Reports;

public class GetMedicineConsumptionQueryHandler(IMedicalDbContext db)
    : IRequestHandler<GetMedicineConsumptionQuery, Result<MedicineConsumptionDto>>
{
    public async Task<Result<MedicineConsumptionDto>> Handle(GetMedicineConsumptionQuery request, CancellationToken ct)
    {
        var saleItemsQuery = db.SaleItems
            .Join(db.Sales,
                  item => item.SaleId,
                  sale => sale.Id,
                  (item, sale) => new { item, sale })
            .Where(x => x.sale.CompanyId == request.CompanyId
                     && x.sale.CreatedAt >= request.From
                     && x.sale.CreatedAt < request.To.AddDays(1));

        if (request.LocationId.HasValue)
            saleItemsQuery = saleItemsQuery.Where(x => x.sale.LocationId == request.LocationId.Value);

        var saleItems = await saleItemsQuery
            .Select(x => new { x.item.ProductId, x.item.Quantity, x.item.UnitPrice })
            .ToListAsync(ct);

        var productIds = saleItems.Select(i => i.ProductId).Distinct().ToList();
        var medicines = await db.Products
            .Where(p => productIds.Contains(p.Id) && p.Kind == ProductKind.FinalProduct)
            .Select(p => new { p.Id, p.Name, p.SKU })
            .ToDictionaryAsync(p => p.Id, ct);

        var result = saleItems
            .Where(i => medicines.ContainsKey(i.ProductId))
            .GroupBy(i => i.ProductId)
            .Select(g =>
            {
                var prod = medicines[g.Key];
                return new MedicineUsageDto(
                    g.Key, prod.Name, prod.SKU,
                    g.Sum(i => i.Quantity),
                    g.Sum(i => i.Quantity * i.UnitPrice));
            })
            .OrderByDescending(m => m.QuantityDispensed)
            .ToList();

        return Result<MedicineConsumptionDto>.Success(new MedicineConsumptionDto(request.From, request.To, result));
    }
}
