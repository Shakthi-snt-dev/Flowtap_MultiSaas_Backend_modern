using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class StockBatch : TenantEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public string? BatchNumber { get; set; }
    public int Quantity { get; set; }
    public decimal CostPrice { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
