using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flowtap_Infrastructure.BackgroundJobs;

public class ReorderCheckService(
    IServiceScopeFactory scopeFactory,
    ILogger<ReorderCheckService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckReorderLevelsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during reorder level check");
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task CheckReorderLevelsAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        // Load active rules with product name for notifications
        var rules = await db.ReorderRules
            .Include(r => r.Product)
            .Where(r => r.IsActive)
            .ToListAsync(ct);

        foreach (var rule in rules)
        {
            var stock = await db.WarehouseStocks
                .FirstOrDefaultAsync(s =>
                    s.ProductId == rule.ProductId &&
                    s.WarehouseId == rule.WarehouseId, ct);

            var currentQty = stock?.Quantity ?? 0m;

            if (currentQty > rule.MinimumQuantity)
                continue;   // stock is fine — skip

            // Skip if an unhandled alert already exists for this product+warehouse
            var alertExists = await db.ReorderAlerts
                .AnyAsync(a =>
                    a.ProductId  == rule.ProductId &&
                    a.WarehouseId == rule.WarehouseId &&
                    !a.IsHandled, ct);

            if (alertExists)
                continue;

            // ── 1. Determine severity ────────────────────────────────────────
            var severity = currentQty == 0
                ? ReorderAlertSeverity.Critical
                : currentQty <= rule.MinimumQuantity / 2
                    ? ReorderAlertSeverity.Warning
                    : ReorderAlertSeverity.Low;

            // ── 2. Create the alert ──────────────────────────────────────────
            var alert = new ReorderAlert
            {
                CompanyId       = rule.CompanyId,
                ProductId       = rule.ProductId,
                WarehouseId     = rule.WarehouseId,
                ReorderRuleId   = rule.Id,
                CurrentQuantity = (int)currentQty,
                ReorderLevel    = (int)rule.MinimumQuantity,
                Severity        = severity,
                IsHandled       = false,
            };
            db.ReorderAlerts.Add(alert);

            logger.LogInformation(
                "Reorder alert created for Product {ProductId} in Warehouse {WarehouseId} (Severity: {Severity})",
                rule.ProductId, rule.WarehouseId, severity);

            // ── 3. Check per-location auto-reorder setting ───────────────────
            var locationSettings = rule.LocationId.HasValue
                ? await db.LocationInventorySettings
                    .FirstOrDefaultAsync(s =>
                        s.CompanyId  == rule.CompanyId &&
                        s.LocationId == rule.LocationId.Value, ct)
                : null;

            var autoReorderEnabled       = locationSettings?.EnableAutoReorder ?? false;
            var notificationEmail        = locationSettings?.ReorderNotificationEmail;

            // ── 4. Auto-create Purchase Order (if enabled + supplier known) ──
            Guid? purchaseOrderId = null;
            if (autoReorderEnabled && rule.PreferredSupplierId.HasValue)
            {
                var poNumber = await GeneratePoNumberAsync(db, ct);

                var po = new PurchaseOrder
                {
                    CompanyId             = rule.CompanyId,
                    WarehouseId           = rule.WarehouseId,
                    LocationId            = rule.LocationId,
                    SupplierId            = rule.PreferredSupplierId.Value,
                    PONumber              = poNumber,
                    Status                = PurchaseOrderStatus.Draft,
                    Currency              = "INR",
                    ExpectedDeliveryDate  = rule.LeadTimeDays.HasValue
                                               ? DateTime.UtcNow.AddDays(rule.LeadTimeDays.Value)
                                               : null,
                    InternalNotes         = $"Auto-generated by reorder rule (Product: {rule.Product.Name})",
                    CreatedByEmployeeId   = Guid.Empty,   // system-generated
                    SubTotal              = 0,
                    TaxAmount             = 0,
                    TotalAmount           = 0,
                };

                var item = new PurchaseOrderItem
                {
                    ProductId  = rule.ProductId,
                    Quantity   = (int)rule.ReorderQuantity,
                    UnitCost   = 0,   // supplier will confirm price on receipt
                    TaxPercent = 0,
                };

                db.PurchaseOrders.Add(po);
                purchaseOrderId = po.Id;
                alert.LinkedPurchaseOrderId = po.Id;

                logger.LogInformation(
                    "Auto-created Purchase Order {PONumber} for Product {ProductId}",
                    poNumber, rule.ProductId);
            }

            // ── 5. Queue email notification ───────────────────────────────────
            if (!string.IsNullOrWhiteSpace(notificationEmail))
            {
                var severityLabel = severity.ToString();
                var poNote = purchaseOrderId.HasValue
                    ? "A draft Purchase Order has been created automatically."
                    : "Please raise a Purchase Order manually.";

                var emailBody = $"""
                    <h3>Reorder Alert — {severityLabel}</h3>
                    <p><strong>Product:</strong> {rule.Product.Name}</p>
                    <p><strong>Current Stock:</strong> {currentQty}</p>
                    <p><strong>Minimum Level:</strong> {rule.MinimumQuantity}</p>
                    <p><strong>Reorder Qty:</strong> {rule.ReorderQuantity}</p>
                    <p>{poNote}</p>
                    <p style="color:#888;font-size:12px">This is an automated message from Flowtap Inventory.</p>
                    """;

                db.NotificationQueues.Add(new NotificationQueue
                {
                    CompanyId = rule.CompanyId,
                    Type      = "Email",
                    Recipient = notificationEmail,
                    Subject   = $"[{severityLabel}] Reorder Alert: {rule.Product.Name} is low on stock",
                    Payload   = emailBody,
                    Status    = "Pending",
                });
            }
        }

        await db.SaveChangesAsync(ct);
    }

    // Generates the next PO number: PO-000001, PO-000002, …
    private static async Task<string> GeneratePoNumberAsync(IApplicationDbContext db, CancellationToken ct)
    {
        var count = await db.PurchaseOrders.CountAsync(ct);
        return $"PO-{count + 1:D6}";
    }
}
