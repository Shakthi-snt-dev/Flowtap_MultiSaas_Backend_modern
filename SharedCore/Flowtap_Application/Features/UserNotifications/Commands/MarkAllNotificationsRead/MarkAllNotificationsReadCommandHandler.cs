using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.UserNotifications.Commands.MarkAllNotificationsRead;

public class MarkAllNotificationsReadCommandHandler(IApplicationDbContext db)
    : IRequestHandler<MarkAllNotificationsReadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkAllNotificationsReadCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var unread = await db.UserNotifications
            .Where(n => n.CompanyId == request.CompanyId
                     && n.UserId == request.UserId
                     && !n.IsRead)
            .ToListAsync(ct);

        foreach (var n in unread)
        {
            n.IsRead = true;
            n.ReadAt = now;
        }

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
