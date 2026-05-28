using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class InventorySerialLocation : TenantEntity
{
    public Guid SerialId { get; set; }
    public InventorySerial Serial { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public Guid RackId { get; set; }
    public WarehouseRack Rack { get; set; } = null!;
    public Guid BinId { get; set; }
    public WarehouseBin Bin { get; set; } = null!;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
