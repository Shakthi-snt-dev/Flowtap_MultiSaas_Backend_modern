using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Notifications.Commands.MarkMessageAsRead;

public class MarkMessageAsReadCommandHandler(IApplicationDbContext db)
    : IRequestHandler<MarkMessageAsReadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkMessageAsReadCommand request, CancellationToken ct)
    {
        var message = await db.DirectMessages
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.RecipientId == request.UserId, ct);

        if (message is null)
            return Result<bool>.Failure("Message not found.");

        if (!message.IsRead)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }

        return Result<bool>.Success(true);
    }
}
