using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Common.Notifications;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Food.Application.StockAlerts;
using Flowtap_Food.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flowtap_Food.Application.Notifications;

/// <summary>
/// Triggered immediately after stock is deducted for a food sale.
/// Checks StockAlertRules (food-only) for the sold products and queues
/// Email / SMS / WhatsApp notifications if any raw material falls below its threshold.
///
/// Registered ONLY in Flowtap_Food_API — silently absent from all other APIs.
/// Complements FoodStockCheckService (every 30 min) with per-sale immediacy.
/// The 4-hour cooldown on LastTriggeredAt prevents duplicate spam even if
/// multiple sales in quick succession trigger the same alert.
/// </summary>
public class FoodStockCheckOnSaleHandler(
    IFoodDbContext foodDb,
    IApplicationDbContext coreDb,
    ILogger<FoodStockCheckOnSaleHandler> logger)
    : INotificationHandler<SaleStockDeductedNotification>
{
    private static readonly TimeSpan NotificationCooldown = TimeSpan.FromHours(4);

    public async Task Handle(SaleStockDeductedNotification notification, CancellationToken ct)
    {
        // Check ALL active rules for this company after every sale —
        // not just rules matching the sold products.
        //
        // Why: stock alert rules are often for RawMaterials (ingredients) which are
        // not sold directly. Their stock may already be below threshold before/after
        // the sale. We want to catch these immediately rather than waiting 30 min
        // for the background job.
        //
        // The 4-hour cooldown (LastTriggeredAt) prevents duplicate spam.
        var rules = await foodDb.StockAlertRules
            .Where(r => r.CompanyId == notification.CompanyId && r.IsActive)
            .ToListAsync(ct);

        if (rules.Count == 0) return;

        var now = DateTime.UtcNow;

        foreach (var rule in rules)
        {
            // Respect cooldown — avoid spamming the same recipient repeatedly
            if (rule.LastTriggeredAt.HasValue &&
                now - rule.LastTriggeredAt.Value < NotificationCooldown)
                continue;

            var stock = await coreDb.WarehouseStocks
                .FirstOrDefaultAsync(s =>
                    s.ProductId   == rule.ProductId     &&
                    s.WarehouseId == rule.WarehouseId   &&
                    s.CompanyId   == notification.CompanyId, ct);

            var currentQty = stock?.Quantity ?? 0m;

            if (stock is null)
            {
                logger.LogDebug(
                    "No stock record for Product {ProductId} in Warehouse {WarehouseId} — check rule warehouse matches where stock is stored",
                    rule.ProductId, rule.WarehouseId);
            }

            if (currentQty >= rule.Threshold)
                continue;   // stock is fine — no alert needed

            // Get product name + kind for message and role resolution
            var product = await coreDb.Products
                .Select(p => new { p.Id, p.Name, Kind = p.Kind.ToString() })
                .FirstOrDefaultAsync(p => p.Id == rule.ProductId, ct);

            var productName = product?.Name ?? rule.ProductId.ToString();
            var productKind = product?.Kind ?? "RawMaterial";

            logger.LogInformation(
                "Food stock alert triggered on sale for Product {ProductName} ({Kind}): {CurrentQty} < {Threshold} {Unit}",
                productName, productKind, currentQty, rule.Threshold, rule.Unit);

            // ── Resolve contacts (each carries its role label) ────────────────
            var roleContacts = await StockAlertRoleResolver.ResolveAsync(rule, coreDb, ct);

            var explicitEmails = rule.GetEmailRecipients().Select(e => new ResolvedContact(e, "Additional")).ToList();
            var explicitPhones = rule.GetSmsRecipients().Select(p => new ResolvedContact(p, "Additional")).ToList();
            var explicitWa     = rule.GetWhatsAppRecipients().Select(p => new ResolvedContact(p, "Additional")).ToList();

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
                                    $"({currentQty} {rule.Unit} remaining, threshold: {rule.Threshold} {rule.Unit}). " +
                                    $"Please restock.",
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
                                    $"Current: {currentQty} {rule.Unit}\nThreshold: {rule.Threshold} {rule.Unit}\n" +
                                    $"Please restock immediately.",
                        Status    = "Pending",
                    });
            }

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
