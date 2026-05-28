using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class Product : TenantEntity
{
    public Guid CategoryId { get; set; }
    public ProductCategory Category { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public ProductKind Kind { get; set; }
    public string SKU { get; set; } = string.Empty;
    public Guid? TaxSlabId { get; set; }
    public string? HsnCode { get; set; }
    public decimal DefaultCostPrice { get; set; }
    public decimal DefaultSalePrice { get; set; }
    public bool IsSerialized { get; set; }
    public bool IsUniversal { get; set; }
    public new bool IsActive { get; set; } = true;
    public string? Tag { get; set; }
    public string? Emoji { get; set; }
    public string? IndustryMetadata { get; set; }
    public ProductPublishStatus PublishStatus { get; set; } = ProductPublishStatus.Draft;
    public ICollection<ProductMedia> Media { get; set; } = [];
    public ICollection<ProductVariant> Variants { get; set; } = [];
    public ICollection<WarehouseStock> WarehouseStocks { get; set; } = [];
}

