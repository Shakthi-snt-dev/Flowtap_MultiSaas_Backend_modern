using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class PermissionCategory : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public ICollection<Permission> Permissions { get; set; } = [];
}
