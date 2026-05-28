using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;

public class PurchaseReturnItem : BaseEntity
{
    public Guid PurchaseReturnId { get; set; }
    public PurchaseReturn PurchaseReturn { get; set; } = null!;
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TaxPercent { get; set; }
}
