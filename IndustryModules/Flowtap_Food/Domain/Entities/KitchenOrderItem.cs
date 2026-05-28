using Flowtap_Domain.SharedKernel;

namespace Flowtap_Food.Domain.Entities;

public class KitchenOrderItem : AuditableEntity
{
    public Guid KitchenOrderId { get; set; }
    public KitchenOrder KitchenOrder { get; set; } = null!;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }    // special instructions, e.g. "no onion"
}
