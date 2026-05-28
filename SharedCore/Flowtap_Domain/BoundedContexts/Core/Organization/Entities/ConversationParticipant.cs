using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

public class ConversationParticipant : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }
    public bool IsActive { get; set; } = true;
}
