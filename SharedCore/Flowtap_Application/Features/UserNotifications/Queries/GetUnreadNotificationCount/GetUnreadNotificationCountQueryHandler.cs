using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.UserNotifications.Queries.GetUnreadNotificationCount;

public class GetUnreadNotificationCountQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetUnreadNotificationCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetUnreadNotificationCountQuery request, CancellationToken ct)
    {
        var count = await db.UserNotifications
            .CountAsync(n => n.CompanyId == request.CompanyId
                          && n.UserId == request.UserId
                          && !n.IsRead, ct);

        return Result<int>.Success(count);
    }
}
