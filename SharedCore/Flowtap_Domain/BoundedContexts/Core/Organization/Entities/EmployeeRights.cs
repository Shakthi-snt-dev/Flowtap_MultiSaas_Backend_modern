using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class EmployeeRights : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public bool General { get; set; }
    public bool Tickets { get; set; }
    public bool Shop { get; set; }
    public bool Clients { get; set; }
    public bool Payments { get; set; }
    public bool Services { get; set; }
    public bool Inventory { get; set; }
    public bool Suppliers { get; set; }
    public bool Salary { get; set; }
    public bool Reports { get; set; }
    public bool Tasks { get; set; }
    public bool Files { get; set; }
    public bool Settings { get; set; }
}
