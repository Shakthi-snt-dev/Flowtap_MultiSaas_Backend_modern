using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

public class DirectMessage : BaseEntity
{
    public Guid   CompanyId            { get; set; }
    public Guid   SenderId             { get; set; }  // UserAccount.Id
    public Guid   RecipientId          { get; set; }  // UserAccount.Id
    public string Subject              { get; set; } = string.Empty;
    public string Body                 { get; set; } = string.Empty;
    public bool   IsRead               { get; set; } = false;
    public bool   IsComplaint          { get; set; } = false;
    public bool   IsDeletedBySender    { get; set; } = false;
    public bool   IsDeletedByRecipient { get; set; } = false;
    public DateTime  CreatedAt         { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt            { get; set; }
}
