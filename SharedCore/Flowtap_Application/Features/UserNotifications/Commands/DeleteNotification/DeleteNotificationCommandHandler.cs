using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.UserNotifications.Commands.DeleteNotification;

public class DeleteNotificationCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteNotificationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteNotificationCommand request, CancellationToken ct)
    {
        var notification = await db.UserNotifications
            .FirstOrDefaultAsync(n => n.Id == request.Id && n.UserId == request.UserId, ct);

        if (notification == null) return Result<bool>.Failure("Notification not found.");

        db.UserNotifications.Remove(notification);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
