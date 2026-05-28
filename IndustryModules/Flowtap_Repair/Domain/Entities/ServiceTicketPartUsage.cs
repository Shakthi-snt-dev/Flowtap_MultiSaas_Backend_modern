using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class ServiceTicketPartUsage : TenantEntity
{
    public Guid ServiceTicketId { get; set; }
    public ServiceTicket ServiceTicket { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;
}
