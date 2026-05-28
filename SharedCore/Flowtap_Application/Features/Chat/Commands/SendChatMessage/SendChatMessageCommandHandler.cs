using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Chat.Commands.SendChatMessage;

public class SendChatMessageCommandHandler(IApplicationDbContext db)
    : IRequestHandler<SendChatMessageCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SendChatMessageCommand request, CancellationToken ct)
    {
        // Verify user is participant
        var isParticipant = await db.ConversationParticipants
            .AnyAsync(p => p.ConversationId == request.ConversationId
                        && p.UserId == request.SenderId
                        && p.IsActive, ct);

        if (!isParticipant)
            return Result<Guid>.Failure("You are not a participant in this conversation.");

        // Resolve sender name from UserProfile when the JWT claim is missing or "Unknown"
        var senderName = request.SenderName;
        if (string.IsNullOrWhiteSpace(senderName) || senderName == "Unknown")
        {
            senderName = await db.UserProfiles
                .Where(p => p.UserAccountId == request.SenderId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(ct) ?? senderName ?? "User";
        }

        var message = new ChatMessage
        {
            ConversationId = request.ConversationId,
            SenderId       = request.SenderId,
            SenderName     = senderName,
            Body           = request.Body,
            IsDeleted      = false,
            CreatedAt      = DateTime.UtcNow,
        };
        db.ChatMessages.Add(message);

        // Update conversation UpdatedAt
        var conversation = await db.Conversations.FindAsync([request.ConversationId], ct);
        if (conversation != null)
            conversation.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(message.Id);
    }
}
