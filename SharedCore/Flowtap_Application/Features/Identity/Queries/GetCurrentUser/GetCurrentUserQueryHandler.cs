using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Identity.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return Result<UserDto>.Failure("Not authenticated.");

        var account = await db.UserAccounts
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Core.Identity.Entities.UserAccount), currentUser.UserId);

        Guid? defaultLocationId = null;
        if (account.AccountType == Flowtap_Domain.BoundedContexts.Core.Identity.Enums.AccountType.Owner || 
            account.AccountType == Flowtap_Domain.BoundedContexts.Core.Identity.Enums.AccountType.Admin)
        {
            var appUser = await db.AppUsers.FirstOrDefaultAsync(a => a.UserAccountId == account.Id, ct);
            defaultLocationId = appUser?.DefaultLocationId;
        }
        else
        {
            var employee = await db.Employees.FirstOrDefaultAsync(e => e.UserAccountId == account.Id, ct);
            defaultLocationId = employee?.DefaultLocationId;
        }

        var userDto = mapper.Map<UserDto>(account) with { DefaultLocationId = defaultLocationId };
        return Result<UserDto>.Success(userDto);
    }
}
