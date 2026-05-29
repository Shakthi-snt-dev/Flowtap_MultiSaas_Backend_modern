using Flowtap_Domain.SharedKernel;

namespace Flowtap_Food.Domain.Entities;

/// <summary>
/// Per-product low-stock alert rule for the food industry kitchen pantry.
/// When WarehouseStock.Quantity falls below Threshold, notifications are sent.
/// </summary>
public class StockAlertRule : TenantEntity
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal Threshold { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool SendEmail { get; set; }
    public bool SendSms { get; set; }
    public bool SendWhatsApp { get; set; }

    /// <summary>
    /// Comma-separated email addresses for email notifications.
    /// e.g. "owner@restaurant.com,chef@restaurant.com,manager@restaurant.com"
    /// </summary>
    public string? EmailRecipients { get; set; }

    /// <summary>
    /// Comma-separated E.164 phone numbers for SMS notifications.
    /// e.g. "+919876543210,+919123456789"
    /// </summary>
    public string? SmsRecipients { get; set; }

    /// <summary>
    /// Comma-separated E.164 phone numbers for WhatsApp notifications.
    /// e.g. "+919876543210,+919123456789"
    /// </summary>
    public string? WhatsAppRecipients { get; set; }

    /// <summary>
    /// Comma-separated role names to notify.
    /// Supported: Owner | Chef | Manager | Admin | WarehouseManager | StoreManager
    /// The system finds all employees with each role at the linked store/warehouse
    /// and notifies them via whatever contact info they have registered.
    /// e.g. "Owner,Chef,Manager"
    /// </summary>
    public string? NotifyRoles { get; set; }

    public DateTime? LastTriggeredAt { get; set; }

    // ── Helpers ──────────────────────────────────────────────────────────────
    public IEnumerable<string> GetEmailRecipients()    => Split(EmailRecipients);
    public IEnumerable<string> GetSmsRecipients()      => Split(SmsRecipients);
    public IEnumerable<string> GetWhatsAppRecipients() => Split(WhatsAppRecipients);
    public IEnumerable<string> GetNotifyRoles()        => Split(NotifyRoles);

    private static IEnumerable<string> Split(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
