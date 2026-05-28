using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Domain.SharedKernel;
using Flowtap_Food.Domain.Enums;

namespace Flowtap_Food.Domain.Entities;

/// <summary>
/// Kitchen Order Ticket (KOT) — created automatically when a food sale is completed.
/// </summary>
public class KitchenOrder : TenantEntity
{
    public Guid LocationId { get; set; }
    public Guid? SaleId { get; set; }
    public Guid? TableId { get; set; }
    public FoodTable? Table { get; set; }
    public FoodOrderType OrderType { get; set; } = FoodOrderType.DineIn;
    public KOTStatus Status { get; set; } = KOTStatus.New;
    public string? KotNumber { get; set; }                // e.g. "KOT-0001"
    public string? Notes { get; set; }
    public DateTime? PreparedAt { get; set; }
    public DateTime? ServedAt { get; set; }
    public ICollection<KitchenOrderItem> Items { get; set; } = [];
}
