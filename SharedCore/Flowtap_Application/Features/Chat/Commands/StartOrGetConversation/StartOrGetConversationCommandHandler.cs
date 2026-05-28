using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Chat.Commands.StartOrGetConversation;

public class StartOrGetConversationCommandHandler(IApplicationDbContext db)
    : IRequestHandler<StartOrGetConversationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(StartOrGetConversationCommand request, CancellationToken ct)
    {
        // Check if a 1-on-1 conversation already exists between these two users
        var existing = await db.ConversationParticipants
            .Where(p => p.UserId == request.CurrentUserId && p.IsActive)
            .Select(p => p.ConversationId)
            .Intersect(
                db.ConversationParticipants
                    .Where(p => p.UserId == request.OtherUserId && p.IsActive)
                    .Select(p => p.ConversationId))
            .Join(db.Conversations,
                  id => id,
                  c => c.Id,
                  (id, c) => c)
            .Where(c => !c.IsGroup && c.CompanyId == request.CompanyId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync(ct);

        if (existing != Guid.Empty)
            return Result<Guid>.Success(existing);

        // Create new conversation — stamp the store context so GET can filter by location
        var conversation = new Conversation
        {
            CompanyId  = request.CompanyId,
            LocationId = request.LocationId,
            IsGroup    = false,
            CreatedAt  = DateTime.UtcNow,
            UpdatedAt  = DateTime.UtcNow,
        };
        db.Conversations.Add(conversation);

        db.ConversationParticipants.Add(new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId         = request.CurrentUserId,
            JoinedAt       = DateTime.UtcNow,
            IsActive       = true,
        });
        db.ConversationParticipants.Add(new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId         = request.OtherUserId,
            JoinedAt       = DateTime.UtcNow,
            IsActive       = true,
        });

        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(conversation.Id);
    }
}
