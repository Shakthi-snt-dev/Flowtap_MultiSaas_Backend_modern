using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IApplicationDbContext db,
    IEmailService emailService,
    IDateTimeService dateTime)
    : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var account = await db.UserAccounts
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower(), ct);

        // Always return success to prevent email enumeration
        if (account is null) return Result<bool>.Success(true);

        var otp = new Random().Next(100000, 999999).ToString();
        account.OTP = otp;
        account.OTPExpiresAt = dateTime.UtcNow.AddMinutes(15);

        await db.SaveChangesAsync(ct);
        await emailService.SendTemplatedAsync(account.Email, "PasswordReset", new { OTP = otp }, ct);

        return Result<bool>.Success(true);
    }
}
