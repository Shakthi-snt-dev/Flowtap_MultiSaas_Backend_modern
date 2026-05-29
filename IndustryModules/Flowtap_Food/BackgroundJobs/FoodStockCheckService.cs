using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Food.Application.StockAlerts;
using Flowtap_Food.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flowtap_Food.BackgroundJobs;

/// <summary>
/// Runs every 30 minutes and checks kitchen StockAlertRules.
/// When a raw material falls below its threshold it queues Email / SMS / WhatsApp
/// notifications to the configured recipient.
/// Distinct from the shared ReorderCheckService (which handles general reorder rules
/// and auto-purchase-orders). This service is food-only and focuses on kitchen pantry
/// notifications — no PO creation, just instant alerts to the kitchen team.
/// </summary>
public class FoodStockCheckService(
    IServiceScopeFactory scopeFactory,
    ILogger<FoodStockCheckService> logger) : BackgroundService
{
    // How often to check — 30 minutes is a good balance for kitchen operations
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(30);

    // Minimum gap between repeat notifications for the same product (avoid spam)
    private static readonly TimeSpan NotificationCooldown = TimeSpan.FromHours(4);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckKitchenStockAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during food kitchen stock check");
            }
            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    private async Task CheckKitchenStockAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();

        // IFoodDbContext gives us StockAlertRules (food-specific)
        var foodDb = scope.ServiceProvider.GetRequiredService<IFoodDbContext>();

        // IApplicationDbContext gives us WarehouseStocks + NotificationQueues (shared core)
        var coreDb = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var rules = await foodDb.StockAlertRules
            .Where(r => r.IsActive)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;

        foreach (var rule in rules)
        {
            // Skip if we already notified recently (cooldown window)
            if (rule.LastTriggeredAt.HasValue &&
                now - rule.LastTriggeredAt.Value < NotificationCooldown)
                continue;

            var stock = await coreDb.WarehouseStocks
                .FirstOrDefaultAsync(s =>
                    s.ProductId  == rule.ProductId &&
                    s.WarehouseId == rule.WarehouseId, ct);

            var currentQty = stock?.Quantity ?? 0m;

            if (currentQty >= rule.Threshold)
                continue;   // stock is fine

            // ── Get product name + kind for the notification message ────────────
            var product = await coreDb.Products
                .Select(p => new { p.Id, p.Name, Kind = p.Kind.ToString() })
                .FirstOrDefaultAsync(p => p.Id == rule.ProductId, ct);

            var productName = product?.Name ?? rule.ProductId.ToString();
            var productKind = product?.Kind ?? "RawMaterial";

            logger.LogInformation(
                "Food stock alert triggered for Product {ProductName} ({Kind}): {CurrentQty} < {Threshold} {Unit}",
                productName, productKind, currentQty, rule.Threshold, rule.Unit);

            // ── Resolve contacts from the rule's NotifyRoles (each contact carries its role label) ──
            var roleContacts = await StockAlertRoleResolver.ResolveAsync(rule, coreDb, ct);

            // Explicit recipients (manually typed) — label them "Additional"
            var explicitEmails = rule.GetEmailRecipients()
                .Select(e => new ResolvedContact(e, "Additional")).ToList();
            var explicitPhones = rule.GetSmsRecipients()
                .Select(p => new ResolvedContact(p, "Additional")).ToList();
            var explicitWa = rule.GetWhatsAppRecipients()
                .Select(p => new ResolvedContact(p, "Additional")).ToList();

            // Merge explicit + role contacts, deduplicate by address
            var emailList = explicitEmails
                .Concat(rule.SendEmail ? roleContacts.Emails : [])
                .DistinctBy(c => c.Address.ToLowerInvariant()).ToList();
            var phoneList = explicitPhones
                .Concat(rule.SendSms ? roleContacts.Phones : [])
                .DistinctBy(c => c.Address.ToLowerInvariant()).ToList();
            var waList = explicitWa
                .Concat(rule.SendWhatsApp ? roleContacts.Phones : [])
                .DistinctBy(c => c.Address.ToLowerInvariant()).ToList();

            // ── Queue one notification per recipient per enabled channel ──────────
            if (rule.SendEmail)
            {
                foreach (var contact in emailList)
                    coreDb.NotificationQueues.Add(new NotificationQueue
                    {
                        CompanyId = rule.CompanyId,
                        Type      = "Email",
                        Recipient = contact.Address,
                        Subject   = $"[Kitchen Alert | {contact.RoleLabel}] {productName} is running low",
                        Payload   = BuildEmailBody(productName, currentQty, rule.Threshold, rule.Unit, contact.RoleLabel),
                        Status    = "Pending",
                    });
            }

            if (rule.SendSms)
            {
                foreach (var contact in phoneList)
                    coreDb.NotificationQueues.Add(new NotificationQueue
                    {
                        CompanyId = rule.CompanyId,
                        Type      = "Sms",
                        Recipient = contact.Address,
                        Subject   = $"[Kitchen Alert | {contact.RoleLabel}]",
                        Payload   = $"Kitchen Alert ({contact.RoleLabel}): {productName} is low " +
                                    $"({currentQty} {rule.Unit} remaining, threshold: {rule.Threshold} {rule.Unit}). Please restock.",
                        Status    = "Pending",
                    });
            }

            if (rule.SendWhatsApp)
            {
                foreach (var contact in waList)
                    coreDb.NotificationQueues.Add(new NotificationQueue
                    {
                        CompanyId = rule.CompanyId,
                        Type      = "WhatsApp",
                        Recipient = contact.Address,
                        Subject   = $"[Kitchen Alert | {contact.RoleLabel}]",
                        Payload   = $"🚨 *Kitchen Stock Alert* ({contact.RoleLabel})\n*{productName}* is running low.\n" +
                                    $"Current: {currentQty} {rule.Unit}\nThreshold: {rule.Threshold} {rule.Unit}\nPlease restock immediately.",
                        Status    = "Pending",
                    });
            }

            // ── Update last triggered timestamp ──────────────────────────────
            rule.LastTriggeredAt = now;
        }

        // Save notification queue entries + updated LastTriggeredAt timestamps
        await coreDb.SaveChangesAsync(ct);
        await foodDb.SaveChangesAsync(ct);
    }

    private static string BuildEmailBody(string productName, decimal current, decimal threshold, string unit, string roleLabel = "") => $"""
        <h3 style="color:#e65c00;">🚨 Kitchen Stock Alert</h3>
        <table style="border-collapse:collapse;font-family:sans-serif;">
          <tr><td style="padding:4px 12px 4px 0;color:#555;">Product</td><td><strong>{productName}</strong></td></tr>
          <tr><td style="padding:4px 12px 4px 0;color:#555;">Current Stock</td><td><strong style="color:#c0392b;">{current} {unit}</strong></td></tr>
          <tr><td style="padding:4px 12px 4px 0;color:#555;">Alert Threshold</td><td>{threshold} {unit}</td></tr>
        </table>
        <p style="margin-top:16px;">Please restock this item before the next service begins.</p>
        <p style="color:#aaa;font-size:11px;">Automated alert from Flowtap Kitchen Management</p>
        """;
}
