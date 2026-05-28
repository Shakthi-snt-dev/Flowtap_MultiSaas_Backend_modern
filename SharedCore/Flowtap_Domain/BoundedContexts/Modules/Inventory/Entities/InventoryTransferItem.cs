using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class InventoryTransferItem : TenantEntity
{
    public Guid InventoryTransferId { get; set; }
    public InventoryTransfer InventoryTransfer { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal ShippedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public TransferItemStatus Status { get; set; } = TransferItemStatus.Pending;
    public ItemCondition Condition { get; set; } = ItemCondition.Good;
}
