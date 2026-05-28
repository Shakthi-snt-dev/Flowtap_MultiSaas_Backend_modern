using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

public class AdminBroadcast : BaseEntity
{
    public Guid CompanyId   { get; set; }
    public Guid? LocationId { get; set; }   // null = company-wide (all stores)
    public string Subject   { get; set; } = string.Empty;
    public string Message   { get; set; } = string.Empty;
    /// <summary>Information | Warning | Alert</summary>
    public string Severity  { get; set; } = "Information";
    public string SentBy    { get; set; } = string.Empty;
    public bool IsActive    { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ── Announcement fields ──────────────────────────────────────────────────
    /// <summary>Banner | Popup | Notification</summary>
    public string Type       { get; set; } = "Notification";
    /// <summary>AllStores | SelectedStores | Role</summary>
    public string TargetType { get; set; } = "AllStores";
    public string? TargetRole { get; set; }
    /// <summary>Low | Normal | High | Critical</summary>
    public string Priority   { get; set; } = "Normal";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate   { get; set; }

    public ICollection<AnnouncementTarget> Targets { get; set; } = [];
}
