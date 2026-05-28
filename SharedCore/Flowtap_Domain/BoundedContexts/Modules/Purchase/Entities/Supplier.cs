using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;

public class Supplier : TenantEntity
{
    public Guid? LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? GSTIN { get; set; }
    public string? Address { get; set; }
    public new bool IsActive { get; set; } = true;
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = [];
}

