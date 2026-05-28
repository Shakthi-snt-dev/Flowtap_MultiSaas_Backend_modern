using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
public class RefreshToken : BaseEntity
{
    public Guid UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
