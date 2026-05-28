using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Chat.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Chat.Queries.GetChatMessages;

public class GetChatMessagesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetChatMessagesQuery, Result<List<ChatMessageDto>>>
{
    public async Task<Result<List<ChatMessageDto>>> Handle(
        GetChatMessagesQuery request, CancellationToken ct)
    {
        // Admins can view any conversation; employees must be participants
        if (!request.IsAdmin)
        {
            var isParticipant = await db.ConversationParticipants
                .AnyAsync(p => p.ConversationId == request.ConversationId
                            && p.UserId == request.UserId
                            && p.IsActive, ct);

            if (!isParticipant)
                return Result<List<ChatMessageDto>>.Failure("You are not a participant in this conversation.");
        }

        var rawMessages = await db.ChatMessages
            .Where(m => m.ConversationId == request.ConversationId)
            .OrderBy(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        // Batch-resolve sender names from UserProfiles (fixes stored "Unknown" entries)
        var senderIds = rawMessages.Select(m => m.SenderId).Distinct().ToList();
        var nameMap   = await db.UserProfiles
            .Where(p => senderIds.Contains(p.UserAccountId))
            .ToDictionaryAsync(p => p.UserAccountId, p => p.Name ?? "", ct);

        var messages = rawMessages.Select(m =>
        {
            var resolvedName = nameMap.TryGetValue(m.SenderId, out var pn) && !string.IsNullOrWhiteSpace(pn)
                ? pn
                : (string.IsNullOrWhiteSpace(m.SenderName) || m.SenderName == "Unknown" ? "User" : m.SenderName);

            return new ChatMessageDto(
                m.Id, m.SenderId, resolvedName,
                m.IsDeleted ? "[Message deleted]" : m.Body,
                m.IsDeleted, m.CreatedAt,
                m.SenderId == request.UserId);
        }).ToList();

        return Result<List<ChatMessageDto>>.Success(messages);
    }
}
