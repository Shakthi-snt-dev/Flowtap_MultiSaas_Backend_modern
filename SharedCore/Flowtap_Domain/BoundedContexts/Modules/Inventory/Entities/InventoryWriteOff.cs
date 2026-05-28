using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class InventoryWriteOff : TenantEntity
{
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string WriteOffNumber { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public InventoryWriteOffType Type { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid RequestedByEmployeeId { get; set; }
    public bool RequiresApproval { get; set; }
    public WriteOffStatus Status { get; set; } = WriteOffStatus.Pending;
    public Guid? ApprovedByEmployeeId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public ICollection<InventoryWriteOffAttachment> Attachments { get; set; } = [];
}
