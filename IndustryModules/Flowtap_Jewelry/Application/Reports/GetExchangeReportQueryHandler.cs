using Flowtap_Application.Common.DTOs;
using Flowtap_Jewelry.Application.Reports.DTOs;
using Flowtap_Jewelry.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Jewelry.Application.Reports;

public class GetExchangeReportQueryHandler(IJewelryDbContext db)
    : IRequestHandler<GetExchangeReportQuery, Result<ExchangeReportDto>>
{
    public async Task<Result<ExchangeReportDto>> Handle(GetExchangeReportQuery request, CancellationToken ct)
    {
        var query = db.MetalExchangeTransactions
            .Where(e => e.CompanyId == request.CompanyId
                     && e.CreatedAt >= request.From
                     && e.CreatedAt < request.To.AddDays(1));
        if (request.LocationId.HasValue)
            query = query.Where(e => e.LocationId == request.LocationId.Value);

        var exchanges = await query
            .Select(e => new
            {
                e.Id, e.ClientName, e.MetalType, e.Purity,
                e.WeightGrams, e.RatePerGram, e.TotalValue, e.CreatedAt
            })
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);

        var items = exchanges.Select(e => new ExchangeItemDto(
            e.Id, e.ClientName, e.MetalType.ToString(), e.Purity.ToString(),
            e.WeightGrams, e.RatePerGram, e.TotalValue, e.CreatedAt
        )).ToList();

        return Result<ExchangeReportDto>.Success(new ExchangeReportDto(
            From: request.From,
            To: request.To,
            TotalExchanges: exchanges.Count,
            TotalWeightGrams: exchanges.Sum(e => e.WeightGrams),
            TotalExchangeValue: exchanges.Sum(e => e.TotalValue),
            Items: items
        ));
    }
}
