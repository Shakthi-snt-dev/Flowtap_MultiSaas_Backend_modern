using Flowtap_Domain.SharedKernel;
using Flowtap_Jewelry.Domain.Enums;

namespace Flowtap_Jewelry.Domain.Entities;

public class MetalRate : TenantEntity
{
    public MetalType MetalType { get; set; }
    public Purity Purity { get; set; }
    public decimal RatePerGram { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    public string? Source { get; set; }
}
