using Flowtap_Domain.BoundedContexts.Modules.Purchase.Enums;
using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;

public class PurchaseOrder : TenantEntity
{
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Guid? LocationId { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public DateTime? StatusUpdatedAt { get; set; }
    public string? ShippingMethod { get; set; }
    public string? CarrierName { get; set; }
    public string? TrackingNumber { get; set; }
    public string? InternalNotes { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    public string? SupplierInvoiceNumber { get; set; }
    public Guid CreatedByEmployeeId { get; set; }
    public ICollection<PurchaseOrderItem> Items { get; set; } = [];
    public ICollection<PurchaseReturn> Returns { get; set; } = [];
}
