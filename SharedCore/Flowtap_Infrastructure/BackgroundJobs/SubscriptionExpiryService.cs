using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Subscription.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flowtap_Infrastructure.BackgroundJobs;

public class SubscriptionExpiryService(
    IServiceScopeFactory scopeFactory,
    ILogger<SubscriptionExpiryService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredSubscriptionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing subscription expiry");
            }
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task ProcessExpiredSubscriptionsAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var now = DateTime.UtcNow;

        var expiredSubs = await db.CompanySubscriptions
            .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate < now)
            .ToListAsync(ct);

        foreach (var sub in expiredSubs)
        {
            sub.Status = SubscriptionStatus.Expired;
            sub.IsActive = false;
            logger.LogInformation(
                "Subscription {Id} for company {CompanyId} marked as expired",
                sub.Id, sub.CompanyId);
        }

        var expiredTrials = await db.TrialPlans
            .Where(t => !t.IsExpired && !t.IsConverted && t.TrialEndDate < now)
            .ToListAsync(ct);

        foreach (var trial in expiredTrials)
        {
            trial.IsExpired = true;
            logger.LogInformation(
                "Trial for company {CompanyId} marked as expired",
                trial.CompanyId);
        }

        if (expiredSubs.Count > 0 || expiredTrials.Count > 0)
            await db.SaveChangesAsync(ct);
    }
}
