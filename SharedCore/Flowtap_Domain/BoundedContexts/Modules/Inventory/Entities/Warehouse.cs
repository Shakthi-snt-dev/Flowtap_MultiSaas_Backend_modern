using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class Warehouse : TenantEntity
{
    public Guid? LocationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public WarehouseType Type { get; set; }
    public Guid? ManagerEmployeeId { get; set; }
    public string? Address { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public WarehouseStatus Status { get; set; } = WarehouseStatus.Active;
    public new bool IsActive { get; set; } = true;
    public WorkingDays WorkingDays { get; set; }
    public TimeSpan? OpenFrom { get; set; }
    public TimeSpan? OpenTo { get; set; }
    public decimal? MaxCapacityKg { get; set; }
    public decimal? CurrentCapacityKg { get; set; }
    public int? MaxCapacityUnits { get; set; }
    public int? CurrentCapacityUnits { get; set; }
    public bool HasRackSystem { get; set; } = false;
    public ICollection<WarehouseStock> Stocks { get; set; } = [];
    public ICollection<WarehouseRack> Racks { get; set; } = [];
}

