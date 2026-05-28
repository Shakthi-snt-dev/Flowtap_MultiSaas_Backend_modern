using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class EmployeeStatusPermission : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public Guid StatusPermissionId { get; set; }
    public StatusPermission StatusPermission { get; set; } = null!;
    public bool CanView { get; set; }
    public bool CanSet { get; set; }
    public string? Color { get; set; }
}
