using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.CreateOffer;

public class CreateOfferCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateOfferCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOfferCommand request, CancellationToken ct)
    {
        var exists = await db.Offers
            .AnyAsync(o => o.CompanyId == request.CompanyId
                && o.PromoCode == request.PromoCode.ToUpper(), ct);
        if (exists)
            return Result<Guid>.Failure($"Promo code '{request.PromoCode}' already exists.");

        var offer = new Offer
        {
            CompanyId = request.CompanyId,
            PromoCode = request.PromoCode.ToUpper(),
            DiscountPercent = request.DiscountPercent,
            MinOrderValue = request.MinOrderValue,
            UsageLimit = request.UsageLimit,
            UsageCount = 0,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            IsActive = true
        };

        db.Offers.Add(offer);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(offer.Id);
    }
}
