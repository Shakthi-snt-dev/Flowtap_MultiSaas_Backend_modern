using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class InventoryTransaction : TenantEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public InventoryTransactionType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal QuantityBefore { get; set; }
    public decimal QuantityAfter { get; set; }
    public decimal CostPrice { get; set; }
    public string Reference { get; set; } = string.Empty;
    public Guid? RelatedEntityId { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}
