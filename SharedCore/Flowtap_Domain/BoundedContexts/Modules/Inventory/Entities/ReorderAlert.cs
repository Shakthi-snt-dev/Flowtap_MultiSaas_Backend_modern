using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class ReorderAlert : TenantEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public Guid? LocationId { get; set; }
    public int CurrentQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public ReorderAlertSeverity Severity { get; set; }
    public Guid ReorderRuleId { get; set; }
    public ReorderRule ReorderRule { get; set; } = null!;
    public bool IsHandled { get; set; }
    public Guid? LinkedPurchaseOrderId { get; set; }
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
