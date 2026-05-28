using Flowtap_Domain.SharedKernel;
using Flowtap_Food.Domain.Enums;

namespace Flowtap_Food.Domain.Entities;

public class FoodTable : TenantEntity
{
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;      // e.g. "Table 1", "Window Seat A"
    public int Capacity { get; set; }
    public string? Section { get; set; }                  // e.g. "Indoor", "Outdoor", "VIP"
    public FoodTableStatus Status { get; set; } = FoodTableStatus.Available;
    public Guid? CurrentSaleId { get; set; }
    public ICollection<KitchenOrder> KitchenOrders { get; set; } = [];
}
