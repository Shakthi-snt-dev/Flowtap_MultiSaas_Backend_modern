using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.UserNotifications.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.UserNotifications.Queries.GetUserNotifications;

public class GetUserNotificationsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetUserNotificationsQuery, Result<List<UserNotificationDto>>>
{
    public async Task<Result<List<UserNotificationDto>>> Handle(
        GetUserNotificationsQuery request, CancellationToken ct)
    {
        var query = db.UserNotifications
            .Where(n => n.CompanyId == request.CompanyId && n.UserId == request.UserId);

        if (request.IsRead.HasValue)
            query = query.Where(n => n.IsRead == request.IsRead.Value);

        var results = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new UserNotificationDto(
                n.Id, n.Type, n.Title, n.Message,
                n.ReferenceId, n.ReferenceType,
                n.IsRead, n.ReadAt, n.CreatedAt))
            .ToListAsync(ct);

        return Result<List<UserNotificationDto>>.Success(results);
    }
}
