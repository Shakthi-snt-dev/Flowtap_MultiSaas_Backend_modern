using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class ProductCategory : TenantEntity
{
    public Guid? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public bool IsSubCategoryExist { get; set; }
    public bool IsDirectProductExist { get; set; }
    public bool IsBrandExist { get; set; }
    public new bool IsActive { get; set; } = true;
    public ICollection<ProductCategory> SubCategories { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
}

