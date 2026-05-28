using Flowtap_Domain.BoundedContexts.Modules.Purchase.Enums;
using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;

public class PurchaseOrderItem : AuditableEntity
{
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TaxPercent { get; set; }
    public PurchaseOrderItemStatus Status { get; set; } = PurchaseOrderItemStatus.Pending;
    public DateTime? LastReceivedAt { get; set; }
}
