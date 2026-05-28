using System.Security.Claims;

namespace Flowtap_Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, Guid? tenantId, string email, IEnumerable<string> roles, IEnumerable<Claim>? extraClaims = null);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
}
