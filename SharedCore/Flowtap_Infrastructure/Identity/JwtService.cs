using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Flowtap_Infrastructure.Identity;

public class JwtService(IOptions<JwtSettings> jwtOptions) : IJwtService
{
    private readonly JwtSettings _settings = jwtOptions.Value;

    public string GenerateAccessToken(Guid userId, Guid? tenantId, string email, IEnumerable<string> roles, IEnumerable<Claim>? extraClaims = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (tenantId.HasValue)
            claims.Add(new Claim("tenantId", tenantId.Value.ToString()));

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        if (extraClaims is not null)
            claims.AddRange(extraClaims);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }

    public Guid? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        if (principal is null) return null;

        var subClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)
                    ?? principal.FindFirst(ClaimTypes.NameIdentifier);

        return subClaim is not null && Guid.TryParse(subClaim.Value, out var id) ? id : null;
    }
}
