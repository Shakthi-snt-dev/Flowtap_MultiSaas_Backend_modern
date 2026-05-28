using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class Employee : AuditableEntity
{
    public Guid UserAccountId { get; set; }
    public Guid CompanyId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public string? VATIN { get; set; }
    public string? Role { get; set; }          // Admin | Manager | Technician | Cashier | Staff etc.
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public DateTime? JoinedAt { get; set; }
    public string? Comment { get; set; }
    public Guid? DefaultLocationId { get; set; }
    // Flat salary fields (simple HR reference — not payroll engine)
    public decimal? Salary { get; set; }
    public string? SalaryType { get; set; }    // Monthly | Weekly | Daily | Hourly
    public string? SalaryCurrency { get; set; }
    public SalarySetting? SalarySetting { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public string? AccessPin { get; set; }
    public ICollection<EmployeeStatusPermission> StatusPermissions { get; set; } = [];
    public ICollection<EmployeeLocationAccess> LocationAccess { get; set; } = [];
    public ICollection<EmployeePermission> Permissions { get; set; } = [];
}
