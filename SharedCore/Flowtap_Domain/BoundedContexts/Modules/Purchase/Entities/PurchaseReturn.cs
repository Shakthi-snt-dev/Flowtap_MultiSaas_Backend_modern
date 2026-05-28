using Flowtap_Domain.BoundedContexts.Modules.Purchase.Enums;
using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;

public class PurchaseReturn : TenantEntity
{
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ReturnNumber { get; set; } = string.Empty;
    public PurchaseReturnStatus Status { get; set; } = PurchaseReturnStatus.Draft;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<PurchaseReturnItem> Items { get; set; } = [];
}
