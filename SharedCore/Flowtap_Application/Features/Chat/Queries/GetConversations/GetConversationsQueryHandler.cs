using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Chat.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Chat.Queries.GetConversations;

public class GetConversationsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetConversationsQuery, Result<List<ConversationSummaryDto>>>
{
    public async Task<Result<List<ConversationSummaryDto>>> Handle(
        GetConversationsQuery request, CancellationToken ct)
    {
        // Get relevant conversation IDs:
        // - Admin/Owner (ViewAll=true): all conversations in the company
        // - Employee: only conversations this user participates in
        List<Guid> myConversationIds;
        if (request.ViewAll)
        {
            myConversationIds = await db.Conversations
                .Where(c => c.CompanyId == request.CompanyId)
                .Select(c => c.Id)
                .ToListAsync(ct);
        }
        else
        {
            myConversationIds = await db.ConversationParticipants
                .Where(p => p.UserId == request.UserId && p.IsActive)
                .Select(p => p.ConversationId)
                .ToListAsync(ct);
        }

        // Filter conversations by store: Conversation.LocationId was stamped at creation time,
        // so this is a single WHERE clause — no employee-join needed.
        var convQuery = db.Conversations
            .Where(c => myConversationIds.Contains(c.Id) && c.CompanyId == request.CompanyId);

        if (request.LocationId.HasValue)
            // Also include conversations with no location stamp (created before this feature existed)
            convQuery = convQuery.Where(c => c.LocationId == request.LocationId.Value || c.LocationId == null);

        var conversations = await convQuery
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync(ct);

        // Batch-load participants for all conversations
        var allParticipants = await db.ConversationParticipants
            .Where(p => myConversationIds.Contains(p.ConversationId) && p.IsActive)
            .ToListAsync(ct);

        // Batch-load last message per conversation
        var lastMessages = await db.ChatMessages
            .Where(m => myConversationIds.Contains(m.ConversationId) && !m.IsDeleted)
            .GroupBy(m => m.ConversationId)
            .Select(g => g.OrderByDescending(m => m.CreatedAt).First())
            .ToListAsync(ct);

        // Batch-load unread counts per conversation for this user
        var myLastSeenMap = allParticipants
            .Where(p => p.UserId == request.UserId)
            .ToDictionary(p => p.ConversationId, p => p.LastSeenAt);

        // Load all user names from profiles
        var userIds = allParticipants.Select(p => p.UserId).Distinct().ToList();
        var userNames = await db.UserProfiles
            .Where(p => userIds.Contains(p.UserAccountId))
            .ToDictionaryAsync(p => p.UserAccountId, p => p.Name ?? "Unknown", ct);

        var result = conversations.Select(conv =>
        {
            var participants = allParticipants.Where(p => p.ConversationId == conv.Id).ToList();
            var lastMsg      = lastMessages.FirstOrDefault(m => m.ConversationId == conv.Id);
            var myLastSeen   = myLastSeenMap.TryGetValue(conv.Id, out var ls) ? ls : null;

            var unreadCount = db.ChatMessages
                .Count(m => m.ConversationId == conv.Id
                         && !m.IsDeleted
                         && m.SenderId != request.UserId
                         && (myLastSeen == null || m.CreatedAt > myLastSeen));

            var participantDtos = participants
                .Select(p => new ParticipantInfoDto(
                    p.UserId,
                    userNames.TryGetValue(p.UserId, out var n) ? n : "Unknown"))
                .ToList();

            // Prefer UserProfile name over stored SenderName (guards against old "Unknown" entries)
            var lastSenderName = lastMsg == null ? "" :
                userNames.TryGetValue(lastMsg.SenderId, out var lsn) ? lsn
                : (string.IsNullOrWhiteSpace(lastMsg.SenderName) || lastMsg.SenderName == "Unknown"
                    ? "User" : lastMsg.SenderName);

            return new ConversationSummaryDto(
                conv.Id,
                conv.Title,
                conv.IsGroup,
                lastMsg?.Body ?? "",
                lastSenderName,
                lastMsg?.CreatedAt ?? conv.CreatedAt,
                unreadCount,
                myLastSeen,
                participantDtos);
        }).ToList();

        return Result<List<ConversationSummaryDto>>.Success(result);
    }
}
