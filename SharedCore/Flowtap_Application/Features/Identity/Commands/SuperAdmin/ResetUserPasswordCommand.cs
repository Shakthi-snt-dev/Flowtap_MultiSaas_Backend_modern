using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Commands.SuperAdmin;

public record ResetUserPasswordCommand(Guid Id, string NewPassword) : IRequest<Result<bool>>;

public class ResetUserPasswordCommandHandler(IApplicationDbContext db, IPasswordHasher hasher)
    : IRequestHandler<ResetUserPasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ResetUserPasswordCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
            return Result<bool>.Failure("Password must be at least 6 characters.");

        var account = await db.UserAccounts
            .FirstOrDefaultAsync(u => u.Id == request.Id, ct);

        if (account is null)
            return Result<bool>.Failure("User account not found.");

        account.PasswordHash = hasher.Hash(request.NewPassword);
        await db.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}
