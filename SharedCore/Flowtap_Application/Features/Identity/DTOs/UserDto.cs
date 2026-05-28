namespace Flowtap_Application.Features.Identity.DTOs;
public record UserDto(Guid Id, string Email, string Name, string? Phone, string? AvatarUrl, bool IsEmailVerified, string AccountType, Guid? DefaultLocationId = null, bool HasPassword = false)
{
    public UserDto() : this(Guid.Empty, string.Empty, string.Empty, null, null, false, string.Empty, null, false) { }
}
public record TokenDto(string AccessToken, string RefreshToken, DateTime ExpiresAt);
public record AuthResponseDto(UserDto User, TokenDto Token);
public record SessionDto(Guid Id, string DeviceInfo, string? IpAddress, DateTime CreatedAt, bool IsCurrent);
