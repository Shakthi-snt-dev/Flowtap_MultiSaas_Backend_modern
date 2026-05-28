using Flowtap_Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flowtap_Infrastructure.BackgroundJobs;

public class EmailDispatchService(
    IServiceScopeFactory scopeFactory,
    ILogger<EmailDispatchService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingEmailsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing notification queue");
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task ProcessPendingEmailsAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var pending = await db.NotificationQueues
            .Where(n => n.Type == "Email" && n.SentAt == null && n.RetryCount < 3)
            .Take(20)
            .ToListAsync(ct);

        foreach (var notification in pending)
        {
            try
            {
                await emailService.SendAsync(
                    notification.Recipient,
                    notification.Subject,
                    notification.Payload,
                    isHtml: true,
                    ct: ct);

                notification.SentAt = DateTime.UtcNow;
                notification.Status = "Sent";
            }
            catch (Exception ex)
            {
                notification.RetryCount++;
                notification.Status = "Failed";
                notification.Error = ex.Message;
                logger.LogWarning(ex, "Failed to send email to {Recipient}", notification.Recipient);
            }
        }

        if (pending.Count > 0)
            await db.SaveChangesAsync(ct);
    }
}
