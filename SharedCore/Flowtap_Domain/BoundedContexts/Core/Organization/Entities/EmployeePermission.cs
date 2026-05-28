using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class EmployeePermission : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
    public bool IsGranted { get; set; }
}
