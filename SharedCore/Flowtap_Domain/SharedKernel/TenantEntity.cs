namespace Flowtap_Domain.SharedKernel;

public abstract class TenantEntity : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public bool IsActive { get; set; } = true;
}
