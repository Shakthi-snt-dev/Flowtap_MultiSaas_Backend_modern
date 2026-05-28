using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class InventorySerial : TenantEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public string? ManufacturerSerial { get; set; }
    public string CompanySerial { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsSold { get; set; }
    public bool IsReturned { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsCancelled { get; set; }
    public DateTime? WarrantyStartDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public InventorySerialLocation? Location { get; set; }
}
