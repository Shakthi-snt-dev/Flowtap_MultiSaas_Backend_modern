using Flowtap_Application.Common.DTOs;
using Flowtap_Jewelry.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Jewelry.Application.MetalRates.GetMetalRates;

public class GetMetalRatesQueryHandler(IJewelryDbContext db)
    : IRequestHandler<GetMetalRatesQuery, Result<List<MetalRateDto>>>
{
    public async Task<Result<List<MetalRateDto>>> Handle(GetMetalRatesQuery request, CancellationToken ct)
    {
        var rates = await db.MetalRates
            .Where(r => r.CompanyId == request.CompanyId && r.IsActive)
            .OrderByDescending(r => r.EffectiveDate)
            .Select(r => new MetalRateDto(
                r.Id, r.MetalType.ToString(), r.Purity.ToString(),
                r.RatePerGram, r.Currency, r.EffectiveDate, r.Source))
            .ToListAsync(ct);

        return Result<List<MetalRateDto>>.Success(rates);
    }
}
