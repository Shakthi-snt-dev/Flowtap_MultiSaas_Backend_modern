using Flowtap_Application.Common.Interfaces;
using Flowtap_Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ApplicationDbContext _context;

    public SubscriptionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanAddEmployee(Guid companyId)
    {
        var subscription = await GetActiveSubscription(companyId);
        if (subscription == null) return false;

        var count = await _context.Employees.CountAsync(e => e.CompanyId == companyId);
        return count < subscription.SubscriptionPlan.MaxEmployees;
    }

    public async Task<bool> CanAddStore(Guid companyId)
    {
        var subscription = await GetActiveSubscription(companyId);
        if (subscription == null) return false;

        var count = await _context.Stores.CountAsync(s => s.CompanyId == companyId);
        return count < subscription.SubscriptionPlan.MaxLocations;
    }

    public async Task<bool> HasFeatureAccess(Guid companyId, string featureCode)
    {
        var subscription = await GetActiveSubscription(companyId);
        if (subscription == null) return false;

        if (string.IsNullOrEmpty(subscription.SubscriptionPlan.FeaturesJson)) return true; // Default to all for now

        var features = System.Text.Json.JsonSerializer.Deserialize<List<string>>(subscription.SubscriptionPlan.FeaturesJson);
        return features?.Contains(featureCode) ?? false;
    }

    private async Task<Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities.CompanySubscription?> GetActiveSubscription(Guid companyId)
    {
        return await _context.CompanySubscriptions
            .Include(s => s.SubscriptionPlan)
            .FirstOrDefaultAsync(s => s.CompanyId == companyId && s.IsActive);
    }
}
