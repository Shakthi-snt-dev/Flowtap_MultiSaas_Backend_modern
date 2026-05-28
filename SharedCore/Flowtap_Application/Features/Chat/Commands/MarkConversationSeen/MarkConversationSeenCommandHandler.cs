using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Chat.Commands.MarkConversationSeen;

public class MarkConversationSeenCommandHandler(IApplicationDbContext db)
    : IRequestHandler<MarkConversationSeenCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkConversationSeenCommand request, CancellationToken ct)
    {
        var participant = await db.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == request.ConversationId
                                   && p.UserId == request.UserId, ct);

        if (participant == null)
            return Result<bool>.Failure("Participant not found.");

        participant.LastSeenAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
