using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class ProductVariant : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public decimal? AdditionalPrice { get; set; }
}
