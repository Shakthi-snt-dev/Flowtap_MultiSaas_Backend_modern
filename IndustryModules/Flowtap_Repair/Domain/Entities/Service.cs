using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class Service : TenantEntity
{
    public Guid? ServiceCategoryId { get; set; }
    public ServiceCategory? ServiceCategory { get; set; }
    public Guid? ProductCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? EstimatedDuration { get; set; }
    public Guid? TaxSlabId { get; set; }
    public string? IconUrl { get; set; }
    public string? Color { get; set; }
    public decimal BasePrice { get; set; }
    public bool RequiresInventory { get; set; }
    public Guid? InventoryProductId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsUniversal { get; set; }
    public ICollection<ServiceDeviceModelMapping> SupportedModels { get; set; } = [];
    public ICollection<ServicePartRequirement> PartRequirements { get; set; } = [];
}
