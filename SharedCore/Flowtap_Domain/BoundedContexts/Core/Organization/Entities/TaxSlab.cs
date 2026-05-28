using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class TaxSlab : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<TaxRateRule> RateRules { get; set; } = [];
}
