using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class EmployeeLocationAccess : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public Guid LocationId { get; set; }
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public bool HasAccess { get; set; } = true;
}
