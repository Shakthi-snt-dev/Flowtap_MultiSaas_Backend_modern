using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.SetProductLocationPrice;

public class SetProductLocationPriceCommandHandler(IApplicationDbContext db)
    : IRequestHandler<SetProductLocationPriceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SetProductLocationPriceCommand request, CancellationToken ct)
    {
        var existing = await db.ProductLocationPrices
            .FirstOrDefaultAsync(p => p.CompanyId == request.CompanyId
                && p.ProductId == request.ProductId
                && p.LocationId == request.LocationId, ct);

        if (existing is not null)
        {
            existing.CostPrice = request.CostPrice;
            existing.SalePrice = request.SalePrice;
            existing.MRP = request.MRP;
            existing.IsTaxIncluded = request.IsTaxIncluded;
            existing.TaxSlabId = request.TaxSlabId;
            existing.EffectiveFrom = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return Result<Guid>.Success(existing.Id);
        }

        var price = new ProductLocationPrice
        {
            CompanyId = request.CompanyId,
            ProductId = request.ProductId,
            LocationId = request.LocationId,
            CostPrice = request.CostPrice,
            SalePrice = request.SalePrice,
            MRP = request.MRP,
            IsTaxIncluded = request.IsTaxIncluded,
            TaxSlabId = request.TaxSlabId,
            Status = PricingStatus.Published,
            EffectiveFrom = DateTime.UtcNow,
            IsActive = true
        };

        db.ProductLocationPrices.Add(price);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(price.Id);
    }
}
