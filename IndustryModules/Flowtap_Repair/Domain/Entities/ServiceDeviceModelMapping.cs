using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class ServiceDeviceModelMapping : BaseEntity
{
    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public Guid DeviceModelId { get; set; }
}
