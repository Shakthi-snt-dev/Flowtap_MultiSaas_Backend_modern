using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Queries.SuperAdmin;

public record TenantSummaryDto(
    Guid Id,
    string Title,
    string BusinessType,
    string Country,
    bool IsActive,
    int StoreCount,
    int UserCount,
    string Plan,
    DateTime CreatedAt);

public record GetAllTenantsQuery : IRequest<Result<List<TenantSummaryDto>>>;

public class GetAllTenantsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllTenantsQuery, Result<List<TenantSummaryDto>>>
{
    public async Task<Result<List<TenantSummaryDto>>> Handle(GetAllTenantsQuery request, CancellationToken ct)
    {
        var tenants = await db.Tenants
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        var tenantIds = tenants.Select(t => t.Id).ToList();

        // Batch-load store counts
        var storeCounts = await db.Stores
            .Where(s => tenantIds.Contains(s.CompanyId) && s.IsActive)
            .GroupBy(s => s.CompanyId)
            .Select(g => new { CompanyId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        // Batch-load user counts (from AppUsers — CompanyId is nullable)
        var userCounts = await db.AppUsers
            .Where(u => u.CompanyId.HasValue && tenantIds.Contains(u.CompanyId.Value))
            .GroupBy(u => u.CompanyId!.Value)
            .Select(g => new { CompanyId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        // Batch-load active subscription plan names (one per company, most recent)
        var subscriptions = await db.CompanySubscriptions
            .Include(s => s.SubscriptionPlan)
            .Where(s => tenantIds.Contains(s.CompanyId) && s.IsActive)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync(ct);

        var result = tenants.Select(t =>
        {
            var storeCount = storeCounts.FirstOrDefault(s => s.CompanyId == t.Id)?.Count ?? 0;
            var userCount  = userCounts.FirstOrDefault(u => u.CompanyId == t.Id)?.Count ?? 0;
            // Pick the most recent subscription for this tenant
            var sub  = subscriptions.Where(s => s.CompanyId == t.Id).FirstOrDefault();
            var plan = sub?.SubscriptionPlan?.Name ?? "Free";

            return new TenantSummaryDto(
                t.Id, t.Title, t.BusinessType, t.Country,
                t.IsActive, storeCount, userCount, plan, t.CreatedAt);
        }).ToList();

        return Result<List<TenantSummaryDto>>.Success(result);
    }
}
