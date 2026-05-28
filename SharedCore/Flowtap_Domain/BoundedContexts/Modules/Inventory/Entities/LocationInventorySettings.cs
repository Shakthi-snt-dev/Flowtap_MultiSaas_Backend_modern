using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class LocationInventorySettings : TenantEntity
{
    public Guid LocationId { get; set; }
    public bool EnableBinTracking { get; set; }
    public bool AllowNegativeStock { get; set; }
    public bool EnableAutoReorder { get; set; }
    public string? ReorderNotificationEmail { get; set; }
}
