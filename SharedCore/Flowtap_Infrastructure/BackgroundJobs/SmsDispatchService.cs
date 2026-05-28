using Flowtap_Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flowtap_Infrastructure.BackgroundJobs;

public class SmsDispatchService(
    IServiceScopeFactory scopeFactory,
    ILogger<SmsDispatchService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingSmsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing SMS notification queue");
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task ProcessPendingSmsAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var smsService = scope.ServiceProvider.GetRequiredService<ISmsService>();

        var pending = await db.NotificationQueues
            .Where(n => n.Type == "Sms" && n.SentAt == null && n.RetryCount < 3)
            .Take(20)
            .ToListAsync(ct);

        foreach (var notification in pending)
        {
            try
            {
                await smsService.SendAsync(
                    notification.Recipient,
                    notification.Payload,
                    ct);

                notification.SentAt = DateTime.UtcNow;
                notification.Status = "Sent";
            }
            catch (Exception ex)
            {
                notification.RetryCount++;
                notification.Status = "Failed";
                notification.Error = ex.Message;
                logger.LogWarning(ex, "Failed to send SMS to {Recipient}", notification.Recipient);
            }
        }

        if (pending.Count > 0)
            await db.SaveChangesAsync(ct);
    }
}
