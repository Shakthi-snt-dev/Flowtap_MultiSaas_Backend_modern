using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class TicketTimeLog : BaseEntity
{
    public Guid TicketId { get; set; }
    public ServiceTicket Ticket { get; set; } = null!;
    public DateTime StartedAt { get; set; }
    public DateTime? StoppedAt { get; set; }
    public Guid StartedByEmployeeId { get; set; }
}
