using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class SerialCounter : TenantEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int LastNumber { get; set; }
}
