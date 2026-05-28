using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class WarehouseStock : TenantEntity
{
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal InTransitQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal MinStockLevel { get; set; }
    public decimal ReorderLevel { get; set; }
    public Guid? PreferredSupplierId { get; set; }
    public DateTime LastMovementAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastPurchasedAt { get; set; }
    public DateTime? LastSoldAt { get; set; }
}
