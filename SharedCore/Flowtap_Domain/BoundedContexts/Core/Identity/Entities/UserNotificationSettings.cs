using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
public class UserNotificationSettings : BaseEntity
{
    public Guid UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; } = null!;
    public bool AssignedToTicketEmail { get; set; } = true;
    public bool AssignedToTicketSms { get; set; }
    public bool TicketStatusChangedPush { get; set; } = true;
    public bool TaskAssignedEmail { get; set; } = true;
    public bool TaskStatusChangedPush { get; set; } = true;
    public bool LowStockAlert { get; set; } = true;
    public bool PaymentReceivedEmail { get; set; } = true;
}
