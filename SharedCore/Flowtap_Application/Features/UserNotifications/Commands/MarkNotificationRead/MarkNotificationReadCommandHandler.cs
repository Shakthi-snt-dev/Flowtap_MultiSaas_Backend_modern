using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.UserNotifications.Commands.MarkNotificationRead;

public class MarkNotificationReadCommandHandler(IApplicationDbContext db)
    : IRequestHandler<MarkNotificationReadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkNotificationReadCommand request, CancellationToken ct)
    {
        var notification = await db.UserNotifications
            .FirstOrDefaultAsync(n => n.Id == request.Id && n.UserId == request.UserId, ct);

        if (notification == null) return Result<bool>.Failure("Notification not found.");

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
