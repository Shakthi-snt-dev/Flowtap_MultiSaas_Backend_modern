using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Identity.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Identity.Queries.GetUserSessions;

public class GetUserSessionsQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<GetUserSessionsQuery, Result<List<SessionDto>>>
{
    public async Task<Result<List<SessionDto>>> Handle(GetUserSessionsQuery request, CancellationToken ct)
    {
        var sessions = await db.UserSessions
            .Where(s => s.UserAccountId == currentUser.UserId && !s.IsRevoked)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

        return Result<List<SessionDto>>.Success(mapper.Map<List<SessionDto>>(sessions));
    }
}
