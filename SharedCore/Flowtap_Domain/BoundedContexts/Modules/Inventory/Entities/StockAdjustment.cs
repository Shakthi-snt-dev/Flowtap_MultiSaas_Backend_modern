using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class StockAdjustment : TenantEntity
{
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string AdjustmentNumber { get; set; } = string.Empty;
    public StockAdjustmentType AdjustmentType { get; set; }
    public decimal QuantityDifference { get; set; }
    public StockAdjustmentReason ReasonCode { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool RequiresApproval { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ApprovedByEmployeeId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public decimal QuantityBefore { get; set; }
    public decimal QuantityAfter { get; set; }
    public Guid? AdjustedByEmployeeId { get; set; }
}
