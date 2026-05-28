namespace Flowtap_Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    Guid? StoreId { get; }
    string? Email { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsOwner { get; }
}
