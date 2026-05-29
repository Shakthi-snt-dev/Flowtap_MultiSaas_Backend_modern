using MediatR;

namespace Flowtap_Application.Common.Notifications;

/// <summary>
/// Published by CreateSaleCommandHandler immediately after stock is deducted for a sale.
///
/// Handlers use this to trigger immediate reorder / kitchen stock checks
/// instead of waiting for the hourly (ReorderCheckService) or 30-minute
/// (FoodStockCheckService) background polling cycle.
///
/// Architecture note: the notification is always published by the shared-core handler.
/// Industry-specific handlers (e.g. FoodStockCheckOnSaleHandler in Flowtap_Food) are
/// registered only in their respective API entry points. MediatR silently skips
/// notification handlers that are not registered — zero impact on other industries.
/// </summary>
public record SaleStockDeductedNotification(
    Guid CompanyId,
    Guid WarehouseId,
    IReadOnlyList<DeductedStockItem> Items) : INotification;

/// <summary>
/// Identifies a product whose stock was deducted by the sale.
/// Handlers query the DB for the current quantity — the deduction has already
/// been committed to the database before this notification is published.
/// </summary>
public record DeductedStockItem(Guid ProductId);
