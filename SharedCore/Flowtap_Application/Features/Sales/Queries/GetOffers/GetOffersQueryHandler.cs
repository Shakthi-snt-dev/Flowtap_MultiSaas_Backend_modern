using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Queries.GetOffers;

public class GetOffersQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetOffersQuery, Result<List<OfferDto>>>
{
    public async Task<Result<List<OfferDto>>> Handle(GetOffersQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var query = db.Offers.Where(o => o.CompanyId == request.CompanyId);

        if (request.ActiveOnly)
            query = query.Where(o => o.IsActive && o.ValidFrom <= now && o.ValidTo >= now);

        var items = await query.OrderByDescending(o => o.CreatedAt).ToListAsync(ct);

        var dtos = items.Select(o => new OfferDto(
            o.Id, o.PromoCode, o.DiscountPercent, o.MinOrderValue,
            o.UsageLimit, o.UsageCount, o.ValidFrom, o.ValidTo, o.IsActive)).ToList();

        return Result<List<OfferDto>>.Success(dtos);
    }
}
