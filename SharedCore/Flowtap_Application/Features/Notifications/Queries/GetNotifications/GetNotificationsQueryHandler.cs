using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Notifications.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetNotificationsQuery, Result<PaginatedList<NotificationListItemDto>>>
{
    public async Task<Result<PaginatedList<NotificationListItemDto>>> Handle(
        GetNotificationsQuery request, CancellationToken ct)
    {
        var query = db.NotificationQueues
            .Where(n => n.CompanyId == request.CompanyId);

        if (!string.IsNullOrWhiteSpace(request.Channel))
            query = query.Where(n => n.Type == request.Channel);

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(n => n.Status == request.Status);

        // Note: Payload is jsonb — LIKE is not supported on jsonb in PostgreSQL.
        // Subject is text and is sufficient: Email alerts always have "[Kitchen Alert]" in Subject.
        // SMS/WhatsApp alerts have empty Subject but are found via the channel filter.
        if (!string.IsNullOrWhiteSpace(request.SubjectContains))
            query = query.Where(n => n.Subject.Contains(request.SubjectContains));

        var totalCount = await query.CountAsync(ct);

        var paged = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationListItemDto(
                n.Id, n.Type, n.Recipient, n.Subject, n.Payload,
                n.Status, n.Error, n.CreatedAt, n.SentAt, n.RetryCount))
            .ToListAsync(ct);

        return Result<PaginatedList<NotificationListItemDto>>.Success(
            new PaginatedList<NotificationListItemDto>(paged, totalCount, request.Page, request.PageSize));
    }
}
