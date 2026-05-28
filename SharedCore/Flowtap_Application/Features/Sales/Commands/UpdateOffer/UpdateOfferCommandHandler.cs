using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.UpdateOffer;

public class UpdateOfferCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateOfferCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOfferCommand request, CancellationToken ct)
    {
        var offer = await db.Offers
            .FirstOrDefaultAsync(o => o.Id == request.Id && o.CompanyId == request.CompanyId, ct);

        if (offer is null)
            return Result<bool>.Failure("Offer not found.");

        if (!string.IsNullOrWhiteSpace(request.PromoCode))
            offer.PromoCode = request.PromoCode.ToUpper();

        if (request.DiscountPercent.HasValue)
            offer.DiscountPercent = request.DiscountPercent.Value;

        if (request.MinOrderValue.HasValue)
            offer.MinOrderValue = request.MinOrderValue.Value;

        if (request.UsageLimit.HasValue)
            offer.UsageLimit = request.UsageLimit.Value;

        if (request.ValidFrom.HasValue)
            offer.ValidFrom = request.ValidFrom.Value;

        if (request.ValidTo.HasValue)
            offer.ValidTo = request.ValidTo.Value;

        if (request.IsActive.HasValue)
            offer.IsActive = request.IsActive.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
