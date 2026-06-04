using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.Application.Reports.DTOs;
using Flowtap_Repair.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Reports;

public class GetPartConsumptionQueryHandler(IRepairDbContext db)
    : IRequestHandler<GetPartConsumptionQuery, Result<PartConsumptionDto>>
{
    public async Task<Result<PartConsumptionDto>> Handle(GetPartConsumptionQuery request, CancellationToken ct)
    {
        var usages = await db.ServiceTicketPartUsages
            .Where(u => u.CompanyId == request.CompanyId
                     && u.UsedAt >= request.From
                     && u.UsedAt < request.To.AddDays(1))
            .Select(u => new { u.ProductId, u.Quantity, u.UnitPrice })
            .ToListAsync(ct);

        var productIds = usages.Select(u => u.ProductId).Distinct().ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Name, p.SKU })
            .ToDictionaryAsync(p => p.Id, ct);

        var parts = usages
            .GroupBy(u => u.ProductId)
            .Select(g =>
            {
                var prod = products.TryGetValue(g.Key, out var p) ? p : null;
                return new PartUsageDto(
                    g.Key,
                    prod?.Name ?? "Unknown",
                    prod?.SKU,
                    g.Sum(u => u.Quantity),
                    g.Sum(u => u.Quantity * u.UnitPrice)
                );
            })
            .OrderByDescending(p => p.TotalUsed)
            .ToList();

        return Result<PartConsumptionDto>.Success(new PartConsumptionDto(request.From, request.To, parts));
    }
}
