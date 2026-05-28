using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
public class UserSession : BaseEntity
{
    public Guid UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; } = null!;
    public string DeviceInfo { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsRevoked { get; set; }
}
