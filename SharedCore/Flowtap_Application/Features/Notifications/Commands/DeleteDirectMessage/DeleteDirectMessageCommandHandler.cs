using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Notifications.Commands.DeleteDirectMessage;

public class DeleteDirectMessageCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteDirectMessageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteDirectMessageCommand request, CancellationToken ct)
    {
        var message = await db.DirectMessages
            .FirstOrDefaultAsync(
                m => m.Id == request.Id &&
                     (m.SenderId == request.UserId || m.RecipientId == request.UserId), ct);

        if (message is null)
            return Result<bool>.Failure("Message not found.");

        if (message.SenderId == request.UserId)
            message.IsDeletedBySender = true;

        if (message.RecipientId == request.UserId)
            message.IsDeletedByRecipient = true;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
