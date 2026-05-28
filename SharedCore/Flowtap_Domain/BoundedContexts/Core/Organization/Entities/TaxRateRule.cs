using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class TaxRateRule : BaseEntity
{
    public Guid TaxConfigurationId { get; set; }
    public TaxConfiguration TaxConfiguration { get; set; } = null!;
    public Guid TaxSlabId { get; set; }
    public TaxSlab TaxSlab { get; set; } = null!;
    public string ComponentName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string? Jurisdiction { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}
