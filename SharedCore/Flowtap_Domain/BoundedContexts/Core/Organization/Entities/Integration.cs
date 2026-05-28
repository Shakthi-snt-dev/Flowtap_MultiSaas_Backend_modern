using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

/// <summary>
/// Represents a third-party integration configured for a company.
/// Stores connection credentials (encrypted in ConfigJson) and status.
/// </summary>
public class Integration : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    /// <summary>Category: 'Payment' | 'Accounting' | 'Communication' | 'Ecommerce' | 'Shipping'</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Unique provider key: 'razorpay', 'stripe', 'qui
    /// quickbooks', 'twilio', etc.</summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>Display name shown in UI</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>JSON blob storing API keys, secrets, webhook URLs, etc.</summary>
    public string? ConfigJson { get; set; }

    /// <summary>Whether this integration is currently active</summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>When the integration was last connected/authenticated</summary>
    public DateTime? ConnectedAt { get; set; }

    /// <summary>Optional webhook URL registered with the provider</summary>
    public string? WebhookUrl { get; set; }

    /// <summary>Last sync/health check result message</summary>
    public string? LastStatusMessage { get; set; }

    /// <summary>Last time a health check or sync was performed</summary>
    public DateTime? LastCheckedAt { get; set; }
}
