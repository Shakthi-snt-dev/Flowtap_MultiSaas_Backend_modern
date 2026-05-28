using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Identity.Entities;
using Flowtap_Domain.BoundedContexts.Core.Identity.Enums;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Commands.Register;

public class RegisterCommandHandler(
    IApplicationDbContext db,
    IPasswordHasher hasher,
    IEmailService emailService,
    IDateTimeService dateTime,
    IBackgroundJobClient backgroundJobs)
    : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var exists = await db.UserAccounts.AnyAsync(u => u.Email == request.Email.ToLower(), ct);
        if (exists) return Result<Guid>.Failure("Email is already registered.");

        var verificationToken = Guid.NewGuid().ToString("N");
        var account = new UserAccount
        {
            Email = request.Email.ToLower(),
            PhoneNumber = request.Phone,
            PasswordHash = hasher.Hash(request.Password),
            AccountType = AccountType.Owner,
            IsEmailVerified = false,
            EmailVerificationToken = verificationToken,
            EmailVerificationTokenExpiresAt = dateTime.UtcNow.AddHours(24),
            Profile = new UserProfile
            {
                Name = request.Name,
                Email = request.Email.ToLower(),
                Phone = request.Phone
            },
            NotificationSettings = new UserNotificationSettings()
        };

        db.UserAccounts.Add(account);
        await db.SaveChangesAsync(ct);

        // Persistent background job for email
        backgroundJobs.Enqueue(() => emailService.SendTemplatedAsync(
            account.Email,
            "VerifyEmail",
            new { Name = request.Name, Token = verificationToken },
            default));

        return Result<Guid>.Success(account.Id);
    }
}
