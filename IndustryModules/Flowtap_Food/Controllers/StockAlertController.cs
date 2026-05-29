using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Food.Application.StockAlerts;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Entities;
using Flowtap_Presentation.Authorization;
using Flowtap_Presentation.Controllers;
using Flowtap_Presentation.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Controllers;

[RequiresIndustry(IndustryType.Food)]
[RequirePermission("Food")]
[Route("api/v1/food/stock-alerts")]
public class StockAlertController(ISender sender, IFoodDbContext db, IApplicationDbContext coreDb) : ApiController(sender)
{
    [HttpGet]
    public async Task<IActionResult> GetAlerts(CancellationToken ct)
    {
        var rules = await db.StockAlertRules
            .Where(a => a.CompanyId == CurrentTenantId && a.IsActive)
            .ToListAsync(ct);

        // Return recipients as arrays so the frontend can render tag chips
        var alerts = rules.Select(a => new
        {
            a.Id,
            a.ProductId,
            a.WarehouseId,
            a.Threshold,
            a.Unit,
            a.SendEmail,
            a.SendSms,
            a.SendWhatsApp,
            EmailRecipients    = a.GetEmailRecipients(),
            SmsRecipients      = a.GetSmsRecipients(),
            WhatsAppRecipients = a.GetWhatsAppRecipients(),
            NotifyRoles        = a.GetNotifyRoles(),
            a.LastTriggeredAt
        });

        return Ok(alerts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAlert([FromBody] CreateStockAlertRequest request, CancellationToken ct)
    {
        var alert = new StockAlertRule
        {
            CompanyId          = CurrentTenantId,
            ProductId          = request.ProductId,
            WarehouseId        = request.WarehouseId,
            Threshold          = request.Threshold,
            Unit               = request.Unit,
            SendEmail          = request.SendEmail,
            SendSms            = request.SendSms,
            SendWhatsApp       = request.SendWhatsApp,
            EmailRecipients    = RecipientHelpers.Join(request.EmailRecipients),
            SmsRecipients      = RecipientHelpers.Join(request.SmsRecipients),
            WhatsAppRecipients = RecipientHelpers.Join(request.WhatsAppRecipients),
            NotifyRoles        = RecipientHelpers.Join(request.NotifyRoles),
        };

        db.StockAlertRules.Add(alert);
        await db.SaveChangesAsync(ct);
        return Ok(alert.Id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAlert(Guid id, [FromBody] CreateStockAlertRequest request, CancellationToken ct)
    {
        var alert = await db.StockAlertRules
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == CurrentTenantId, ct);

        if (alert is null) return NotFound();

        alert.Threshold          = request.Threshold;
        alert.Unit               = request.Unit;
        alert.SendEmail          = request.SendEmail;
        alert.SendSms            = request.SendSms;
        alert.SendWhatsApp       = request.SendWhatsApp;
        alert.EmailRecipients    = RecipientHelpers.Join(request.EmailRecipients);
        alert.SmsRecipients      = RecipientHelpers.Join(request.SmsRecipients);
        alert.WhatsAppRecipients = RecipientHelpers.Join(request.WhatsAppRecipients);
        alert.NotifyRoles        = RecipientHelpers.Join(request.NotifyRoles);

        await db.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAlert(Guid id, CancellationToken ct)
    {
        var alert = await db.StockAlertRules
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == CurrentTenantId, ct);

        if (alert is null) return NotFound();

        alert.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Ok();
    }

    /// <summary>
    /// Immediately triggers the stock check for one rule — bypasses the 4-hour cooldown.
    /// Creates NotificationQueue entries if stock is below threshold and reports what happened.
    /// </summary>
    [HttpPost("{id:guid}/trigger-now")]
    public async Task<IActionResult> TriggerNow(Guid id, CancellationToken ct)
    {
        var rule = await db.StockAlertRules
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == CurrentTenantId && a.IsActive, ct);
        if (rule is null) return NotFound();

        // Read current stock
        var stock = await coreDb.WarehouseStocks
            .FirstOrDefaultAsync(s => s.ProductId == rule.ProductId && s.WarehouseId == rule.WarehouseId, ct);

        var currentQty = stock?.Quantity ?? 0m;

        if (currentQty >= rule.Threshold)
        {
            return Ok(new
            {
                triggered = false,
                reason    = $"Stock is {currentQty} {rule.Unit} — above threshold of {rule.Threshold} {rule.Unit}. No alert sent.",
                currentQty,
                threshold = rule.Threshold,
            });
        }

        // Resolve contacts
        var roleContacts = await StockAlertRoleResolver.ResolveAsync(rule, coreDb, ct);

        if (!roleContacts.Emails.Any() && !roleContacts.Phones.Any() &&
            !rule.GetEmailRecipients().Any() && !rule.GetSmsRecipients().Any() && !rule.GetWhatsAppRecipients().Any())
        {
            return Ok(new
            {
                triggered = false,
                reason    = "Stock is below threshold but NO contacts could be resolved. Check the Preview Recipients for details.",
                currentQty,
                threshold = rule.Threshold,
            });
        }

        // Get product name
        var product = await coreDb.Products.Select(p => new { p.Id, p.Name })
            .FirstOrDefaultAsync(p => p.Id == rule.ProductId, ct);
        var productName = product?.Name ?? rule.ProductId.ToString();

        int queuedCount = 0;

        // Queue Email
        if (rule.SendEmail)
        {
            var emails = rule.GetEmailRecipients().Select(e => new ResolvedContact(e, "Additional"))
                .Concat(roleContacts.Emails).DistinctBy(c => c.Address.ToLowerInvariant()).ToList();
            foreach (var contact in emails)
            {
                coreDb.NotificationQueues.Add(new Flowtap_Domain.BoundedContexts.Core.Organization.Entities.NotificationQueue
                {
                    CompanyId = rule.CompanyId,
                    Type      = "Email",
                    Recipient = contact.Address,
                    Subject   = $"[Kitchen Alert | {contact.RoleLabel}] {productName} is running low",
                    Payload   = $"<p>Stock for <strong>{productName}</strong> is <strong>{currentQty} {rule.Unit}</strong> — below threshold of {rule.Threshold} {rule.Unit}.</p>",
                    Status    = "Pending",
                });
                queuedCount++;
            }
        }

        // Queue SMS / WhatsApp
        if (rule.SendSms || rule.SendWhatsApp)
        {
            var phones = rule.GetSmsRecipients().Select(p => new ResolvedContact(p, "Additional"))
                .Concat(roleContacts.Phones).DistinctBy(c => c.Address.ToLowerInvariant()).ToList();
            foreach (var contact in phones)
            {
                if (rule.SendSms)
                {
                    coreDb.NotificationQueues.Add(new Flowtap_Domain.BoundedContexts.Core.Organization.Entities.NotificationQueue
                    {
                        CompanyId = rule.CompanyId,
                        Type      = "Sms",
                        Recipient = contact.Address,
                        Subject   = $"[Kitchen Alert | {contact.RoleLabel}]",
                        Payload   = $"Kitchen Alert ({contact.RoleLabel}): {productName} is low ({currentQty} {rule.Unit}). Please restock.",
                        Status    = "Pending",
                    });
                    queuedCount++;
                }
                if (rule.SendWhatsApp)
                {
                    coreDb.NotificationQueues.Add(new Flowtap_Domain.BoundedContexts.Core.Organization.Entities.NotificationQueue
                    {
                        CompanyId = rule.CompanyId,
                        Type      = "WhatsApp",
                        Recipient = contact.Address,
                        Subject   = $"[Kitchen Alert | {contact.RoleLabel}]",
                        Payload   = $"🚨 *Kitchen Stock Alert* ({contact.RoleLabel})\n*{productName}* is running low.\nCurrent: {currentQty} {rule.Unit} / Threshold: {rule.Threshold} {rule.Unit}.",
                        Status    = "Pending",
                    });
                    queuedCount++;
                }
            }
        }

        rule.LastTriggeredAt = DateTime.UtcNow;
        await coreDb.SaveChangesAsync(ct);
        await db.SaveChangesAsync(ct);

        return Ok(new
        {
            triggered  = true,
            reason     = $"Stock is {currentQty} {rule.Unit} — below threshold of {rule.Threshold} {rule.Unit}. {queuedCount} notification(s) queued.",
            currentQty,
            threshold  = rule.Threshold,
            queued     = queuedCount,
        });
    }

    /// <summary>
    /// Dry-run the role resolver for a rule — shows exactly who would receive
    /// the next alert and why some contacts may be missing.
    /// </summary>
    [HttpGet("{id:guid}/preview-recipients")]
    public async Task<IActionResult> PreviewRecipients(Guid id, CancellationToken ct)
    {
        var rule = await db.StockAlertRules
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == CurrentTenantId, ct);

        if (rule is null) return NotFound();

        // Resolve contacts and capture warnings via the logger
        var warnings  = new List<string>();
        var contacts  = await StockAlertRoleResolverPreview.ResolveWithDiagnosticsAsync(
            rule, coreDb, warnings, ct);

        return Ok(new
        {
            notifyRoles        = rule.GetNotifyRoles(),
            emailRecipients    = rule.GetEmailRecipients(),
            smsRecipients      = rule.GetSmsRecipients(),
            whatsAppRecipients = rule.GetWhatsAppRecipients(),
            resolvedContacts   = contacts,
            warnings,
        });
    }
}

/// <summary>
/// Recipients are sent as arrays from the frontend (e.g. ["owner@r.com","chef@r.com"]).
/// The controller joins them to comma-separated strings before storing.
/// </summary>
public record CreateStockAlertRequest(
    Guid      ProductId,
    Guid      WarehouseId,
    decimal   Threshold,
    string    Unit,
    bool      SendEmail,
    bool      SendSms,
    bool      SendWhatsApp,
    string[]? EmailRecipients,
    string[]? SmsRecipients,
    string[]? WhatsAppRecipients,
    string[]? NotifyRoles);   // e.g. ["Owner","Chef","Manager","WarehouseManager"]

file static class RecipientHelpers
{
    /// <summary>Joins an array to a comma-separated string; null/empty → null.</summary>
    internal static string? Join(string[]? values) =>
        values is { Length: > 0 }
            ? string.Join(",", values.Select(v => v.Trim()).Where(v => v.Length > 0))
            : null;
}
