using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class Permission : AuditableEntity
{
    public Guid CategoryId { get; set; }
    public PermissionCategory Category { get; set; } = null!;
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<EmployeePermission> EmployeePermissions { get; set; } = [];
}
