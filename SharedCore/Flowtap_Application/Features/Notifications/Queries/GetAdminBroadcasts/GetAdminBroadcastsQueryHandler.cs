using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Notifications.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Notifications.Queries.GetAdminBroadcasts;

public class GetAdminBroadcastsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAdminBroadcastsQuery, Result<List<AdminBroadcastDto>>>
{
    public async Task<Result<List<AdminBroadcastDto>>> Handle(
        GetAdminBroadcastsQuery request, CancellationToken ct)
    {
        var query = db.AdminBroadcasts
            .Where(b => b.CompanyId == request.CompanyId);

        if (request.ActiveOnly)
        {
            query = query.Where(b => b.IsActive);

            // Exclude broadcasts dismissed by this user
            var dismissedIdsQuery = db.UserNotifications
                .Where(n => n.UserId == request.UserId && n.Type == "DismissedBroadcast" && n.ReferenceId != null)
                .Select(n => n.ReferenceId);

            query = query.Where(b => !dismissedIdsQuery.Contains(b.Id));
        }

        if (request.LocationId.HasValue)
        {
            var locId = request.LocationId.Value;
            query = query.Where(b => 
                b.TargetType == "AllStores" || 
                (b.TargetType == "SelectedStores" && b.Targets.Any(t => t.LocationId == locId)) ||
                b.LocationId == locId
            );
        }

        if (!string.IsNullOrWhiteSpace(request.Type))
            query = query.Where(b => b.Type == request.Type);

        var results = await query
            .OrderByDescending(b => b.CreatedAt)
            .Take(request.Limit)
            .Select(b => new AdminBroadcastDto(
                b.Id, b.Subject, b.Message, b.Severity,
                b.SentBy, b.CreatedAt, b.LocationId,
                b.Type, b.TargetType, b.Priority, b.StartDate, b.EndDate))
            .ToListAsync(ct);

        return Result<List<AdminBroadcastDto>>.Success(results);
    }
}
