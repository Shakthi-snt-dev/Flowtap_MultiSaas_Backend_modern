using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class InventoryCostLayer : TenantEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public decimal RemainingQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime ReceivedAt { get; set; }
}
