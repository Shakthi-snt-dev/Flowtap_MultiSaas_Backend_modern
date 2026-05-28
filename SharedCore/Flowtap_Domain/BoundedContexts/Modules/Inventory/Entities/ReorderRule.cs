using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class ReorderRule : TenantEntity
{
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid? LocationId { get; set; }
    public decimal MinimumQuantity { get; set; }
    public decimal ReorderQuantity { get; set; }
    public bool IncludeSafetyStock { get; set; }
    public Guid? PreferredSupplierId { get; set; }
    public int? LeadTimeDays { get; set; }
    public new bool IsActive { get; set; } = true;
    public ICollection<ReorderAlert> Alerts { get; set; } = [];
}

