using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Commands.SuperAdmin;

public record ToggleUserAccountCommand(Guid Id) : IRequest<Result<bool>>;

public class ToggleUserAccountCommandHandler(IApplicationDbContext db)
    : IRequestHandler<ToggleUserAccountCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ToggleUserAccountCommand request, CancellationToken ct)
    {
        var account = await db.UserAccounts
            .FirstOrDefaultAsync(u => u.Id == request.Id, ct);

        if (account is null)
            return Result<bool>.Failure("User account not found.");

        account.IsActive = !account.IsActive;
        await db.SaveChangesAsync(ct);

        return Result<bool>.Success(account.IsActive);
    }
}
