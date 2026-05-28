using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class TaxConfiguration : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public string CountryCode { get; set; } = string.Empty;
    public TaxSystemType SystemType { get; set; }
    public string TaxIdNumber { get; set; } = string.Empty;
    public string? TaxRepresentativeName { get; set; }
    public bool IsTaxExempt { get; set; }
    public bool IsInclusive { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<TaxRateRule> RateRules { get; set; } = [];
}
