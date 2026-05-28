using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class DeviceBrand : AuditableEntity
{
    public Guid? ProductCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<DeviceModel> Models { get; set; } = [];
}
