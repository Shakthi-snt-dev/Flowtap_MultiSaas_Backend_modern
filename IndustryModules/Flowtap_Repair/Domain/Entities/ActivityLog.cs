using Flowtap_Repair.Domain.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class ActivityLog : TenantEntity
{
    public ActivityEntityType EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Details { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? LocationId { get; set; }
}
