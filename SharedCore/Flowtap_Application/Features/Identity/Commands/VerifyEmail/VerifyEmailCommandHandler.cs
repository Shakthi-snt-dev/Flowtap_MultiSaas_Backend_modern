using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Commands.VerifyEmail;

public class VerifyEmailCommandHandler(IApplicationDbContext db, IDateTimeService dateTime)
    : IRequestHandler<VerifyEmailCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(VerifyEmailCommand request, CancellationToken ct)
    {
        var account = await db.UserAccounts
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == request.Token, ct);

        if (account is null)
            return Result<bool>.Failure("Invalid verification token.");

        if (account.EmailVerificationTokenExpiresAt < dateTime.UtcNow)
            return Result<bool>.Failure("Verification token has expired. Please request a new one.");

        account.IsEmailVerified = true;
        account.EmailVerificationToken = null;
        account.EmailVerificationTokenExpiresAt = null;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
