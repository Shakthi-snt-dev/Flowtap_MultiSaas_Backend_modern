using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class InventoryTransfer : TenantEntity
{
    public Guid FromWarehouseId { get; set; }
    public Warehouse FromWarehouse { get; set; } = null!;
    public Guid ToWarehouseId { get; set; }
    public Warehouse ToWarehouse { get; set; } = null!;
    public Guid RequestedByEmployeeId { get; set; }
    public string? TransferNumber { get; set; }
    public TransferStatus Status { get; set; } = TransferStatus.Draft;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? ExpectedArrivalDate { get; set; }
    public string? VehicleNumber { get; set; }
    public string? CourierName { get; set; }
    public string? LRNumber { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ShippingMethod { get; set; }
    public string? Notes { get; set; }
    public Guid? LastUpdatedByEmployeeId { get; set; }
    public ICollection<InventoryTransferItem> Items { get; set; } = [];
}
