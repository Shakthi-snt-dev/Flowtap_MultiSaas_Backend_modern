using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

public class ChatMessage : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = "";
    public string Body { get; set; } = "";
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
