using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class AppUser : AuditableEntity
{
    public Guid UserAccountId { get; set; }
    public Guid? CompanyId { get; set; }
    public Tenant? Company { get; set; }
    public Guid? DefaultLocationId { get; set; }
}
