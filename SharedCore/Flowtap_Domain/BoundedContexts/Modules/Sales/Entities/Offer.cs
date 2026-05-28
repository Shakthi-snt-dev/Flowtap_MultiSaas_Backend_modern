using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class Offer : TenantEntity
{
    public string PromoCode { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public decimal MinOrderValue { get; set; }
    public int UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public new bool IsActive { get; set; } = true;
}

