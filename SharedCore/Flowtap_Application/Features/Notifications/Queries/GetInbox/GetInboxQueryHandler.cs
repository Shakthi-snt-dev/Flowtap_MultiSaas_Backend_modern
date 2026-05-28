using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Notifications.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Notifications.Queries.GetInbox;

public class GetInboxQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetInboxQuery, Result<List<DirectMessageDto>>>
{
    public async Task<Result<List<DirectMessageDto>>> Handle(
        GetInboxQuery request, CancellationToken ct)
    {
        var query = db.DirectMessages
            .Where(m =>
                m.CompanyId   == request.CompanyId &&
                m.RecipientId == request.UserId    &&
                !m.IsDeletedByRecipient);

        if (request.IsComplaint.HasValue)
            query = query.Where(m => m.IsComplaint == request.IsComplaint.Value);

        if (request.LocationId.HasValue)
        {
            var locationUserIds = await db.Employees
                .Where(e => e.CompanyId == request.CompanyId
                         && e.DefaultLocationId == request.LocationId.Value)
                .Select(e => e.UserAccountId)
                .ToListAsync(ct);

            query = query.Where(m => locationUserIds.Contains(m.SenderId) || locationUserIds.Contains(m.RecipientId));
        }

        // Fetch messages with sender names via join on UserProfiles
        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var userIds = messages.SelectMany(m => new[] { m.SenderId, m.RecipientId })
                              .Distinct()
                              .ToList();

        var profileMap = await db.UserProfiles
            .Where(p => userIds.Contains(p.UserAccountId))
            .ToDictionaryAsync(p => p.UserAccountId, p => p.Name, ct);

        var result = messages.Select(m => new DirectMessageDto(
            m.Id,
            m.Subject,
            m.Body,
            m.SenderId,
            profileMap.TryGetValue(m.SenderId, out var sn) ? sn : "Unknown",
            m.RecipientId,
            profileMap.TryGetValue(m.RecipientId, out var rn) ? rn : "Unknown",
            m.IsRead,
            m.IsComplaint,
            m.CreatedAt,
            m.ReadAt
        )).ToList();

        return Result<List<DirectMessageDto>>.Success(result);
    }
}
