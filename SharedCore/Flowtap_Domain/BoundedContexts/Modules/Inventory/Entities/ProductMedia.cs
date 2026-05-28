using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class ProductMedia : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}
