using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IPasswordHasher hasher)
    : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        if (currentUser.UserId is null)
            return Result<bool>.Failure("Not authenticated.");

        var account = await db.UserAccounts
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, ct);

        if (account is null)
            return Result<bool>.Failure("User not found.");

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            return Result<bool>.Failure("Password must be at least 6 characters.");

        // User already has a password — verify the current one first
        if (account.PasswordHash is not null)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                return Result<bool>.Failure("Current password is required.");

            if (!hasher.Verify(request.CurrentPassword, account.PasswordHash))
                return Result<bool>.Failure("Current password is incorrect.");
        }
        // else: no password set yet — allow setting directly (first-time set)

        account.PasswordHash = hasher.Hash(request.NewPassword);
        await db.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}
