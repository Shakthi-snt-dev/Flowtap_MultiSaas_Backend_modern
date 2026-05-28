using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

public class Conversation : BaseEntity
{
    public Guid CompanyId { get; set; }
    public Guid? LocationId { get; set; }  // store where this conversation was initiated
    public string? Title { get; set; }
    public bool IsGroup { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ConversationParticipant> Participants { get; set; } = [];
    public ICollection<ChatMessage> Messages { get; set; } = [];
}
