using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Identity.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IApplicationDbContext db,
    IJwtService jwt,
    IDateTimeService dateTime)
    : IRequestHandler<RefreshTokenCommand, Result<TokenDto>>
{
    public async Task<Result<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var stored = await db.RefreshTokens
            .Include(r => r.UserAccount)
            .FirstOrDefaultAsync(r => r.Token == request.Token, ct);

        if (stored is null || stored.IsRevoked || stored.ExpiresAt < dateTime.UtcNow)
            return Result<TokenDto>.Failure("Invalid or expired refresh token.");

        var appUser = await db.AppUsers
            .FirstOrDefaultAsync(a => a.UserAccountId == stored.UserAccountId, ct);

        var roles = new[] { stored.UserAccount.AccountType.ToString() };
        var newAccess = jwt.GenerateAccessToken(stored.UserAccountId, appUser?.CompanyId, stored.UserAccount.Email, roles);
        var newRefresh = jwt.GenerateRefreshToken();

        stored.IsRevoked = true;
        stored.RevokedAt = dateTime.UtcNow;
        stored.ReplacedByToken = newRefresh;

        var newToken = new Flowtap_Domain.BoundedContexts.Core.Identity.Entities.RefreshToken
        {
            UserAccountId = stored.UserAccountId,
            Token = newRefresh,
            ExpiresAt = dateTime.UtcNow.AddDays(30)
        };
        db.RefreshTokens.Add(newToken);
        await db.SaveChangesAsync(ct);

        return Result<TokenDto>.Success(new TokenDto(newAccess, newRefresh, dateTime.UtcNow.AddHours(1)));
    }
}
