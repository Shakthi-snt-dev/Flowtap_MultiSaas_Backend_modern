using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
public class UserProfile : BaseEntity
{
    public Guid UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}
