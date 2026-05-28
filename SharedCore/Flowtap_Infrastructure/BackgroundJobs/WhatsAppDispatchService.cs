using Flowtap_Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Flowtap_Infrastructure.BackgroundJobs;

/// <summary>
/// Polls NotificationQueues every 30 seconds for Type = "WhatsApp" entries and sends
/// each message using the company's Meta WhatsApp Business credentials from the Integrations table.
///
/// Credential lookup: Integration where Provider = "whatsapp" and CompanyId matches.
/// ConfigJson keys: accessToken, phoneNumberId, businessId.
/// If no integration is found or credentials are missing, the service logs a warning
/// and leaves the message in Pending state (retry logic applies up to 3 attempts).
/// </summary>
public class WhatsAppDispatchService(
    IServiceScopeFactory scopeFactory,
    ILogger<WhatsAppDispatchService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingWhatsAppAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing WhatsApp notification queue");
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task ProcessPendingWhatsAppAsync(CancellationToken ct)
    {
        using var scope     = scopeFactory.CreateScope();
        var db              = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var whatsAppService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();

        var pending = await db.NotificationQueues
            .Where(n => n.Type == "WhatsApp" && n.SentAt == null && n.RetryCount < 3)
            .Take(20)
            .ToListAsync(ct);

        foreach (var notification in pending)
        {
            try
            {
                // Resolve per-company Meta WhatsApp credentials from the Integrations table
                var (accessToken, phoneNumberId) = await ResolveWhatsAppCredsAsync(
                    db, notification.CompanyId, ct);

                await whatsAppService.SendAsync(
                    notification.Recipient,
                    notification.Payload,
                    accessToken,
                    phoneNumberId,
                    ct);

                notification.SentAt = DateTime.UtcNow;
                notification.Status = "Sent";
            }
            catch (Exception ex)
            {
                notification.RetryCount++;
                notification.Status = "Failed";
                notification.Error  = ex.Message;
                logger.LogWarning(ex, "Failed to send WhatsApp to {Recipient}", notification.Recipient);
            }
        }

        if (pending.Count > 0)
            await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Looks up the WhatsApp Business Integration for the given company and extracts
    /// Meta Cloud API credentials from ConfigJson.
    /// Returns empty strings if not configured (WhatsAppService will skip gracefully).
    /// </summary>
    private static async Task<(string AccessToken, string PhoneNumberId)>
        ResolveWhatsAppCredsAsync(IApplicationDbContext db, Guid? companyId, CancellationToken ct)
    {
        if (companyId is null)
            return (string.Empty, string.Empty);

        var integration = await db.Integrations
            .AsNoTracking()
            .FirstOrDefaultAsync(i =>
                i.CompanyId == companyId.Value &&
                i.Provider  == "whatsapp"      &&
                i.IsEnabled, ct);

        if (integration?.ConfigJson is null)
            return (string.Empty, string.Empty);

        var cfg = JsonSerializer.Deserialize<Dictionary<string, string>>(integration.ConfigJson)
                  ?? [];

        cfg.TryGetValue("accessToken",   out var accessToken);
        cfg.TryGetValue("phoneNumberId", out var phoneNumberId);

        return (accessToken ?? string.Empty, phoneNumberId ?? string.Empty);
    }
}
