using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

public class AuditLog : TenantEntity
{
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Update, Delete
    public string? OldValues { get; set; } // JSON serialized
    public string? NewValues { get; set; } // JSON serialized
    public string? ChangedColumns { get; set; }
    public Guid? UserId { get; set; }
    public string? IpAddress { get; set; }
    public Guid? LocationId { get; set; }
}
