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
        // Resolve company employee emails/phones for filtering
        var companyContacts = await db.Employees
            .Where(e => e.CompanyId == request.CompanyId)
            .Join(db.UserProfiles,
                  e => e.UserAccountId,
                  p => p.UserAccountId,
                  (e, p) => new { p.Email, p.Phone })
            .ToListAsync(ct);

        var emails = companyContacts.Select(c => c.Email).Where(e => !string.IsNullOrWhiteSpace(e)).ToHashSet();
        var phones = companyContacts.Select(c => c.Phone).Where(p => !string.IsNullOrWhiteSpace(p)).ToHashSet();

        // Pull all recent notifications — we filter in-memory to avoid complex EF OR queries
        var baseQuery = db.NotificationQueues.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Channel))
            baseQuery = baseQuery.Where(n => n.Type == request.Channel);

        if (!string.IsNullOrWhiteSpace(request.Status))
            baseQuery = baseQuery.Where(n => n.Status == request.Status);

        var allItems = await baseQuery
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationListItemDto(
                n.Id, n.Type, n.Recipient, n.Subject,
                n.Status, n.Error, n.CreatedAt, n.SentAt))
            .ToListAsync(ct);

        // Filter to this company's recipients
        var filtered = allItems
            .Where(n => emails.Contains(n.Recipient) || phones.Contains(n.Recipient))
            .ToList();

        var totalCount = filtered.Count;
        var paged = filtered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Result<PaginatedList<NotificationListItemDto>>.Success(
            new PaginatedList<NotificationListItemDto>(paged, totalCount, request.Page, request.PageSize));
    }
}
