using Flowtap_Application.Common.DTOs;
using Flowtap_Jewelry.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Jewelry.Application.Exchange.GetExchanges;

public class GetExchangesQueryHandler(IJewelryDbContext db)
    : IRequestHandler<GetExchangesQuery, Result<List<MetalExchangeDto>>>
{
    public async Task<Result<List<MetalExchangeDto>>> Handle(GetExchangesQuery request, CancellationToken ct)
    {
        var query = db.MetalExchangeTransactions
            .Where(e => e.CompanyId == request.CompanyId);

        if (request.LocationId.HasValue)
            query = query.Where(e => e.LocationId == request.LocationId.Value);

        var exchanges = await query
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new MetalExchangeDto(
                e.Id, e.LocationId, e.ClientName, e.ExchangeType.ToString(),
                e.MetalType.ToString(), e.Purity.ToString(), e.WeightGrams,
                e.PurityPercent, e.NetWeightGrams, e.RatePerGram, e.TotalValue,
                e.Description, e.ReferenceNumber, e.CreatedAt))
            .ToListAsync(ct);

        return Result<List<MetalExchangeDto>>.Success(exchanges);
    }
}
