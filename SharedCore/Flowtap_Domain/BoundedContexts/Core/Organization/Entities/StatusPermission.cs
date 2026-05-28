using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class StatusPermission : AuditableEntity
{
    public string StatusName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? Color { get; set; }
    public ICollection<EmployeeStatusPermission> EmployeeStatusPermissions { get; set; } = [];
}
