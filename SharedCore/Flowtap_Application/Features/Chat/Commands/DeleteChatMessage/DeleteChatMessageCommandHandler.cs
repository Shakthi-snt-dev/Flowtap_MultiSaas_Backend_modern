using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Chat.Commands.DeleteChatMessage;

public class DeleteChatMessageCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteChatMessageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteChatMessageCommand request, CancellationToken ct)
    {
        var message = await db.ChatMessages
            .FirstOrDefaultAsync(m => m.Id == request.MessageId
                                   && m.SenderId == request.SenderId
                                   && !m.IsDeleted, ct);

        if (message == null)
            return Result<bool>.Failure("Message not found or not owned by you.");

        message.IsDeleted = true;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
