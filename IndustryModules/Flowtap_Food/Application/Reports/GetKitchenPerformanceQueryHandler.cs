using Flowtap_Application.Common.DTOs;
using Flowtap_Food.Application.Reports.DTOs;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.Reports;

public class GetKitchenPerformanceQueryHandler(IFoodDbContext db)
    : IRequestHandler<GetKitchenPerformanceQuery, Result<KitchenPerformanceDto>>
{
    public async Task<Result<KitchenPerformanceDto>> Handle(GetKitchenPerformanceQuery request, CancellationToken ct)
    {
        var query = db.KitchenOrders
            .Where(k => k.CompanyId == request.CompanyId
                     && k.CreatedAt >= request.From
                     && k.CreatedAt < request.To.AddDays(1));

        if (request.LocationId.HasValue)
            query = query.Where(k => k.LocationId == request.LocationId.Value);

        var kots = await query
            .Select(k => new
            {
                k.Status,
                k.CreatedAt,
                k.PreparedAt
            })
            .ToListAsync(ct);

        var completed = kots.Where(k => k.Status == KOTStatus.Served && k.PreparedAt.HasValue).ToList();
        var avgPrepMinutes = completed.Count > 0
            ? completed.Average(k => (k.PreparedAt!.Value - k.CreatedAt).TotalMinutes)
            : 0;

        var byHour = kots
            .GroupBy(k => k.CreatedAt.Hour)
            .Select(g =>
            {
                var completedInHour = g.Where(k => k.Status == KOTStatus.Served && k.PreparedAt.HasValue).ToList();
                var avgPrep = completedInHour.Count > 0
                    ? completedInHour.Average(k => (k.PreparedAt!.Value - k.CreatedAt).TotalMinutes)
                    : 0;
                return new KOTByHourDto(g.Key, g.Count(), Math.Round(avgPrep, 1));
            })
            .OrderBy(h => h.Hour)
            .ToList();

        return Result<KitchenPerformanceDto>.Success(new KitchenPerformanceDto(
            From: request.From,
            To: request.To,
            AvgPrepTimeMinutes: Math.Round(avgPrepMinutes, 1),
            TotalKOTs: kots.Count,
            CompletedKOTs: kots.Count(k => k.Status == KOTStatus.Served),
            CancelledKOTs: kots.Count(k => k.Status == KOTStatus.Cancelled),
            ByHour: byHour
        ));
    }
}
