using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class InventoryWriteOffAttachment : BaseEntity
{
    public Guid InventoryWriteOffId { get; set; }
    public InventoryWriteOff InventoryWriteOff { get; set; } = null!;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
}
