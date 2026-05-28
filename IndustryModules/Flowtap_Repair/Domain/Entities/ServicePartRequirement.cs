using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class ServicePartRequirement : BaseEntity
{
    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
