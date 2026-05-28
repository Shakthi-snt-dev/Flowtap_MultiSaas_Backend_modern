using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Notifications.Commands.DismissAdminBroadcast;

public class DismissAdminBroadcastCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DismissAdminBroadcastCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DismissAdminBroadcastCommand request, CancellationToken ct)
    {
        var broadcast = await db.AdminBroadcasts
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == request.CompanyId, ct);

        if (broadcast is null)
            return Result<bool>.Failure("Broadcast not found.");

        // Record that this specific user has dismissed the broadcast
        bool alreadyDismissed = await db.UserNotifications
            .AnyAsync(n => n.UserId == request.UserId && n.ReferenceId == request.Id && n.Type == "DismissedBroadcast", ct);

        if (!alreadyDismissed)
        {
            db.UserNotifications.Add(new Flowtap_Domain.BoundedContexts.Core.Organization.Entities.UserNotification
            {
                CompanyId = request.CompanyId,
                UserId = request.UserId,
                Type = "DismissedBroadcast",
                ReferenceId = request.Id,
                IsRead = true,
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync(ct);
        }

        return Result<bool>.Success(true);
    }
}
