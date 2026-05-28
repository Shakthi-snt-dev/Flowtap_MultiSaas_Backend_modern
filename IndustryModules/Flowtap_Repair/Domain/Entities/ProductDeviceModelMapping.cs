using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class ProductDeviceModelMapping : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid DeviceModelId { get; set; }
    public DeviceModel DeviceModel { get; set; } = null!;
}
