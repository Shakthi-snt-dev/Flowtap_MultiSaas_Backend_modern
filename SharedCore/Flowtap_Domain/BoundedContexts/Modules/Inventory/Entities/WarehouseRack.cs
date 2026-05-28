using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class WarehouseRack : TenantEntity
{
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ZoneLabel { get; set; }
    public ZoneType? ZoneType { get; set; }
    public RackType? Type { get; set; }
    public decimal? MaxLoadKg { get; set; }
    public decimal? CurrentLoadKg { get; set; }
    public int? Levels { get; set; }
    public new bool IsActive { get; set; } = true;
    public DateTime? LastAuditedAt { get; set; }
    public ICollection<WarehouseBin> Bins { get; set; } = [];
}

