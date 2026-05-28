using Flowtap_Domain.BoundedContexts.Core.Identity.Enums;
using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Core.Identity.Entities;

public class UserAccount : AuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? PasswordHash { get; set; }
    public string? OTP { get; set; }
    public DateTime? OTPExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; set; }
    public AccountType AccountType { get; set; }
    public UserProfile? Profile { get; set; }
    public UserNotificationSettings? NotificationSettings { get; set; }
    public ICollection<UserSession> Sessions { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
