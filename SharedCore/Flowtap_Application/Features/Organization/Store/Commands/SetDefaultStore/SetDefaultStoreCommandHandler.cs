using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Identity.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Flowtap_Application.Features.Organization.Store.Commands.SetDefaultStore;

public class SetDefaultStoreCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<SetDefaultStoreCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(SetDefaultStoreCommand request, CancellationToken ct)
    {
        if (currentUser.UserId == null)
            return Result<Unit>.Failure("Unauthorized.");

        // First check if the user is an owner (in UserAccount)
        var userAccount = await db.UserAccounts
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId.Value, ct);

        if (userAccount == null)
            return Result<Unit>.Failure("User account not found.");

        if (userAccount.AccountType == AccountType.Owner || userAccount.AccountType == AccountType.Admin)
        {
            // Owner/Admin: set DefaultLocationId in AppUser table
            var appUser = await db.AppUsers
                .FirstOrDefaultAsync(a => a.UserAccountId == userAccount.Id, ct);
            if (appUser != null)
            {
                appUser.DefaultLocationId = request.StoreId;
                await db.SaveChangesAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
        }

        // Set DefaultLocationId in Employee table
        var employee = await db.Employees
            .FirstOrDefaultAsync(e => e.UserAccountId == userAccount.Id, ct);

        if (employee != null)
        {
            employee.DefaultLocationId = request.StoreId;
            await db.SaveChangesAsync(ct);
            return Result<Unit>.Success(Unit.Value);
        }

        return Result<Unit>.Failure("Employee or Owner account not found.");
    }
}
