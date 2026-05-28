using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Commands.Logout;

public class LogoutCommandHandler(IApplicationDbContext db, IDateTimeService dateTime)
    : IRequestHandler<LogoutCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken ct)
    {
        var token = await db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken, ct);

        if (token is not null && !token.IsRevoked)
        {
            token.IsRevoked = true;
            token.RevokedAt = dateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }

        return Result<bool>.Success(true);
    }
}
