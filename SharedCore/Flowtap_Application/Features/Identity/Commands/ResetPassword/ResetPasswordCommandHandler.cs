using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    IApplicationDbContext db,
    IPasswordHasher hasher,
    IDateTimeService dateTime)
    : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var account = await db.UserAccounts
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower(), ct);

        if (account is null || account.OTP != request.OTP || account.OTPExpiresAt < dateTime.UtcNow)
            return Result<bool>.Failure("Invalid or expired OTP.");

        account.PasswordHash = hasher.Hash(request.NewPassword);
        account.OTP = null;
        account.OTPExpiresAt = null;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
