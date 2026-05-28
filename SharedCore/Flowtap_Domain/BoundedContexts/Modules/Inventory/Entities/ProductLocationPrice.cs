using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class ProductLocationPrice : TenantEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid LocationId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? MRP { get; set; }
    public PricingStatus Status { get; set; } = PricingStatus.Draft;
    public bool IsTaxIncluded { get; set; }
    public Guid? TaxSlabId { get; set; }    // store-specific tax slab (null = use product default)
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public new bool IsActive { get; set; } = true;
}

