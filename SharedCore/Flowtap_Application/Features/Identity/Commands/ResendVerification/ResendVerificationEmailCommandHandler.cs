using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Commands.ResendVerification;

public class ResendVerificationEmailCommandHandler(
    IApplicationDbContext db,
    IEmailService emailService,
    IDateTimeService dateTime,
    IBackgroundJobClient backgroundJobs)
    : IRequestHandler<ResendVerificationEmailCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ResendVerificationEmailCommand request, CancellationToken ct)
    {
        var account = await db.UserAccounts
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower(), ct);

        // Always return success to prevent email enumeration
        if (account is null) return Result<bool>.Success(true);

        if (account.IsEmailVerified) return Result<bool>.Success(true);

        var token = Guid.NewGuid().ToString("N");
        account.EmailVerificationToken = token;
        account.EmailVerificationTokenExpiresAt = dateTime.UtcNow.AddHours(24);

        await db.SaveChangesAsync(ct);
        
        backgroundJobs.Enqueue(() => emailService.SendTemplatedAsync(
            account.Email, 
            "VerifyEmail", 
            new { Token = token }, 
            default));

        return Result<bool>.Success(true);
    }
}
