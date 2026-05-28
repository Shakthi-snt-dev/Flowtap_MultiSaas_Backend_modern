using Flowtap_Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Flowtap_Infrastructure.BackgroundJobs;

/// <summary>
/// Polls NotificationQueues every 30 seconds for Type = "Sms" entries and sends
/// each message using the company's Twilio credentials from the Integrations table.
///
/// Credential lookup: Integration where Provider = "twilio" and CompanyId matches.
/// ConfigJson keys: accountSid, authToken, fromNumber.
/// If no integration is found or credentials are missing, the service logs a warning
/// and leaves the message in Pending state (retry logic applies up to 3 attempts).
/// </summary>
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
        var db         = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var smsService = scope.ServiceProvider.GetRequiredService<ISmsService>();

        var pending = await db.NotificationQueues
            .Where(n => n.Type == "Sms" && n.SentAt == null && n.RetryCount < 3)
            .Take(20)
            .ToListAsync(ct);

        foreach (var notification in pending)
        {
            try
            {
                // Resolve per-company Twilio credentials from the Integrations table
                var (accountSid, authToken, fromNumber) = await ResolveTwilioCredsAsync(
                    db, notification.CompanyId, ct);

                await smsService.SendAsync(
                    notification.Recipient,
                    notification.Payload,
                    accountSid,
                    authToken,
                    fromNumber,
                    ct);

                notification.SentAt = DateTime.UtcNow;
                notification.Status = "Sent";
            }
            catch (Exception ex)
            {
                notification.RetryCount++;
                notification.Status = "Failed";
                notification.Error  = ex.Message;
                logger.LogWarning(ex, "Failed to send SMS to {Recipient}", notification.Recipient);
            }
        }

        if (pending.Count > 0)
            await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Looks up the Twilio Integration for the given company and extracts credentials
    /// from ConfigJson. Returns empty strings if not configured (SmsService will skip gracefully).
    /// </summary>
    private static async Task<(string AccountSid, string AuthToken, string FromNumber)>
        ResolveTwilioCredsAsync(IApplicationDbContext db, Guid? companyId, CancellationToken ct)
    {
        if (companyId is null)
            return (string.Empty, string.Empty, string.Empty);

        var integration = await db.Integrations
            .AsNoTracking()
            .FirstOrDefaultAsync(i =>
                i.CompanyId == companyId.Value &&
                i.Provider  == "twilio"        &&
                i.IsEnabled, ct);

        if (integration?.ConfigJson is null)
            return (string.Empty, string.Empty, string.Empty);

        var cfg = JsonSerializer.Deserialize<Dictionary<string, string>>(integration.ConfigJson)
                  ?? [];

        cfg.TryGetValue("accountSid", out var accountSid);
        cfg.TryGetValue("authToken",  out var authToken);
        cfg.TryGetValue("fromNumber", out var fromNumber);

        return (accountSid ?? string.Empty, authToken ?? string.Empty, fromNumber ?? string.Empty);
    }
}
