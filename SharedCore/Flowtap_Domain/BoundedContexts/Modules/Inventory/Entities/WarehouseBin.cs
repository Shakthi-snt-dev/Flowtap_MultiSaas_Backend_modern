using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class WarehouseBin : TenantEntity
{
    public Guid RackId { get; set; }
    public WarehouseRack Rack { get; set; } = null!;
    public string Code { get; set; } = string.Empty;
    public int? Level { get; set; }
    public int? Position { get; set; }
    public decimal? MaxWeightKg { get; set; }
    public decimal? CurrentWeightKg { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }
    public new bool IsActive { get; set; } = true;
    public BinStatus Status { get; set; } = BinStatus.Empty;
    public ICollection<WarehouseBinStock> BinStocks { get; set; } = [];
}

