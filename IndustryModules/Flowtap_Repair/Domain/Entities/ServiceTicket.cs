using Flowtap_Repair.Domain.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class ServiceTicket : TenantEntity
{
    public Guid LocationId { get; set; }
    public Guid ClientId { get; set; }
    public string? TicketNumber { get; set; }
    public Guid? PrimaryServiceId { get; set; }
    public TicketType Type { get; set; } = TicketType.Paid;
    public TicketStatus Status { get; set; } = TicketStatus.New;
    public DeviceDetails? DeviceDetails { get; set; }
    public string? Modification { get; set; }
    public string? Appearance { get; set; }
    public string? Password { get; set; }
    public string? Equipment { get; set; }
    public string? Reason { get; set; }
    public string? MastersNotes { get; set; }
    public Guid? ExecutorEmployeeId { get; set; }
    public Guid? ManagerEmployeeId { get; set; }
    public ServiceFinancials? Financials { get; set; }
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public DateTime? Deadline { get; set; }
    public string? PreRepairChecklist { get; set; }
    public string? AccessoryList { get; set; }
    public Guid? SaleId { get; set; }
    public DateTime? ClosedAt { get; set; }
    public byte[]? RowVersion { get; set; }
    public ICollection<ServiceTicketItem> Items { get; set; } = [];
    public ICollection<TicketTimeLog> TimeLogs { get; set; } = [];
}
