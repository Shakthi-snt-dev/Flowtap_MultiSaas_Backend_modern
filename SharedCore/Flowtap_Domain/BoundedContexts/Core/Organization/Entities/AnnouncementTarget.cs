using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

public class AnnouncementTarget : BaseEntity
{
    public Guid AdminBroadcastId { get; set; }
    public AdminBroadcast AdminBroadcast { get; set; } = null!;
    public Guid LocationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
