using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class DeviceModel : AuditableEntity
{
    public Guid? ProductCategoryId { get; set; }
    public Guid BrandId { get; set; }
    public DeviceBrand Brand { get; set; } = null!;
    public Guid? ParentModelId { get; set; }
    public DeviceModel? ParentModel { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<DeviceModel> SubModels { get; set; } = [];
    public ICollection<ProductDeviceModelMapping> ProductMappings { get; set; } = [];
}
