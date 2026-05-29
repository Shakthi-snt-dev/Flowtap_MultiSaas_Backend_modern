using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Common.Notifications;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flowtap_Application.Features.Inventory.Notifications;

/// <summary>
/// Triggered immediately after stock is deducted for a sale.
/// Checks ReorderRules only for the products that were sold — far more
/// efficient than the hourly ReorderCheckService which scans all rules.
///
/// Registered for all industries. No-op if no rules match.
/// </summary>
public class ReorderCheckOnSaleHandler(
    IApplicationDbContext db,
    ILogger<ReorderCheckOnSaleHandler> logger)
    : INotificationHandler<SaleStockDeductedNotification>
{
    public async Task Handle(SaleStockDeductedNotification notification, CancellationToken ct)
    {
        var productIds = notification.Items.Select(i => i.ProductId).Distinct().ToList();

        // Load reorder rules that match any of the sold products in this warehouse
        var rules = await db.ReorderRules
            .Include(r => r.Product)
            .Where(r =>
                r.CompanyId   == notification.CompanyId   &&
                r.WarehouseId == notification.WarehouseId &&
                r.IsActive                                &&
                productIds.Contains(r.ProductId))
            .ToListAsync(ct);

        if (rules.Count == 0) return;

        foreach (var rule in rules)
        {
            var stock = await db.WarehouseStocks
                .FirstOrDefaultAsync(s =>
                    s.ProductId   == rule.ProductId     &&
                    s.WarehouseId == rule.WarehouseId   &&
                    s.CompanyId   == notification.CompanyId, ct);

            var currentQty = stock?.Quantity ?? 0m;

            if (currentQty > rule.MinimumQuantity)
                continue;   // still within safe level — no alert needed

            // Skip if an unhandled alert already exists to avoid duplicates
            var alertExists = await db.ReorderAlerts
                .AnyAsync(a =>
                    a.ProductId   == rule.ProductId     &&
                    a.WarehouseId == rule.WarehouseId   &&
                    !a.IsHandled, ct);

            if (alertExists)
                continue;

            // ── Determine severity ──────────────────────────────────────────
            var severity = currentQty == 0
                ? ReorderAlertSeverity.Critical
                : currentQty <= rule.MinimumQuantity / 2
                    ? ReorderAlertSeverity.Warning
                    : ReorderAlertSeverity.Low;

            // ── Create alert ────────────────────────────────────────────────
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
                "Reorder alert created on sale for Product {ProductId} in Warehouse {WarehouseId} (qty: {Qty}, severity: {Severity})",
                rule.ProductId, rule.WarehouseId, currentQty, severity);

            // ── Per-location auto-reorder setting ───────────────────────────
            var locationSettings = rule.LocationId.HasValue
                ? await db.LocationInventorySettings
                    .FirstOrDefaultAsync(s =>
                        s.CompanyId  == rule.CompanyId &&
                        s.LocationId == rule.LocationId.Value, ct)
                : null;

            var autoReorderEnabled = locationSettings?.EnableAutoReorder ?? false;
            var notificationEmail  = locationSettings?.ReorderNotificationEmail;

            // ── Auto-create Draft Purchase Order (if enabled + supplier known) ─
            Guid? purchaseOrderId = null;
            if (autoReorderEnabled && rule.PreferredSupplierId.HasValue)
            {
                var count    = await db.PurchaseOrders.CountAsync(ct);
                var poNumber = $"PO-{count + 1:D6}";

                var po = new PurchaseOrder
                {
                    CompanyId            = rule.CompanyId,
                    WarehouseId          = rule.WarehouseId,
                    LocationId           = rule.LocationId,
                    SupplierId           = rule.PreferredSupplierId.Value,
                    PONumber             = poNumber,
                    Status               = PurchaseOrderStatus.Draft,
                    Currency             = "INR",
                    ExpectedDeliveryDate = rule.LeadTimeDays.HasValue
                                              ? DateTime.UtcNow.AddDays(rule.LeadTimeDays.Value)
                                              : null,
                    InternalNotes        = $"Auto-generated by reorder rule on sale (Product: {rule.Product.Name})",
                    CreatedByEmployeeId  = Guid.Empty,
                    SubTotal             = 0,
                    TaxAmount            = 0,
                    TotalAmount          = 0,
                };
                db.PurchaseOrders.Add(po);
                purchaseOrderId        = po.Id;
                alert.LinkedPurchaseOrderId = po.Id;

                logger.LogInformation(
                    "Auto-created Purchase Order {PONumber} for Product {ProductId} after sale stock deduction",
                    poNumber, rule.ProductId);
            }

            // ── Queue email notification ────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(notificationEmail))
            {
                var severityLabel = severity.ToString();
                var poNote        = purchaseOrderId.HasValue
                    ? "A draft Purchase Order has been created automatically."
                    : "Please raise a Purchase Order manually.";

                db.NotificationQueues.Add(new NotificationQueue
                {
                    CompanyId = rule.CompanyId,
                    Type      = "Email",
                    Recipient = notificationEmail,
                    Subject   = $"[{severityLabel}] Reorder Alert: {rule.Product.Name} is low on stock",
                    Payload   = $"""
                        <h3>Reorder Alert — {severityLabel}</h3>
                        <p><strong>Product:</strong> {rule.Product.Name}</p>
                        <p><strong>Current Stock:</strong> {currentQty}</p>
                        <p><strong>Minimum Level:</strong> {rule.MinimumQuantity}</p>
                        <p><strong>Reorder Qty:</strong> {rule.ReorderQuantity}</p>
                        <p>{poNote}</p>
                        <p style="color:#888;font-size:12px">This is an automated message from Flowtap Inventory.</p>
                        """,
                    Status    = "Pending",
                });
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
