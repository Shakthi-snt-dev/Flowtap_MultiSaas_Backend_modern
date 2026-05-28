using Flowtap_Repair.Domain.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class ServiceTicketItem : BaseEntity
{
    public Guid TicketId { get; set; }
    public ServiceTicket Ticket { get; set; } = null!;
    public Guid ItemReferenceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public TicketItemType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxPercent { get; set; }
}
