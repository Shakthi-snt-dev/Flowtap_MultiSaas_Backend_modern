using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

public class UserNotification : BaseEntity
{
    public Guid CompanyId { get; set; }
    public Guid UserId { get; set; }
    /// <summary>LowStock | PaymentFailed | OrderPending | TicketUpdated | WriteOffPending | TransferCompleted | Custom</summary>
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public Guid? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
