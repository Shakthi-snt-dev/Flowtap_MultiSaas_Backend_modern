using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class WarehouseBinStock : TenantEntity
{
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public Guid RackId { get; set; }
    public WarehouseRack Rack { get; set; } = null!;
    public Guid BinId { get; set; }
    public WarehouseBin Bin { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
}
