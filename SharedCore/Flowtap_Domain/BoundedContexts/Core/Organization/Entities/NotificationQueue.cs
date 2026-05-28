using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class NotificationQueue : AuditableEntity
{
    /// <summary>Which company queued this notification. Used by dispatch services to look up
    /// per-company Integration credentials (Twilio / WhatsApp Business). Nullable for
    /// backward-compatibility with rows created before this field was added.</summary>
    public Guid? CompanyId { get; set; }
    public string Type { get; set; } = string.Empty; // Email, Sms, WhatsApp, Push
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Sent, Failed
    public int RetryCount { get; set; }
    public string? Error { get; set; }
    public DateTime? SentAt { get; set; }
}
