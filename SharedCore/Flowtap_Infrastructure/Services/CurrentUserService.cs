using System.Security.Claims;
using Flowtap_Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Flowtap_Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User?.FindFirst("sub")?.Value;
            return value is not null && Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid? TenantId
    {
        get
        {
            var value = User?.FindFirst("tenantId")?.Value;
            return value is not null && Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid? StoreId
    {
        get
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                if (httpContext.Request.Headers.TryGetValue("X-Store-Id", out var headerVal) &&
                    Guid.TryParse(headerVal, out var storeId))
                {
                    return storeId;
                }
                if (httpContext.Request.Headers.TryGetValue("storeId", out var headerVal2) &&
                    Guid.TryParse(headerVal2, out var storeId2))
                {
                    return storeId2;
                }
                if (httpContext.Request.Headers.TryGetValue("x-store-id", out var headerVal3) &&
                    Guid.TryParse(headerVal3, out var storeId3))
                {
                    return storeId3;
                }
            }

            var value = User?.FindFirst("storeId")?.Value;
            return value is not null && Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email =>
        User?.FindFirst(ClaimTypes.Email)?.Value
        ?? User?.FindFirst("email")?.Value;

    public IReadOnlyList<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value)
            .Concat(User.FindAll("role").Select(c => c.Value))
            .Distinct()
            .ToList()
        ?? [];

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsOwner => Roles.Contains("Owner");
}
