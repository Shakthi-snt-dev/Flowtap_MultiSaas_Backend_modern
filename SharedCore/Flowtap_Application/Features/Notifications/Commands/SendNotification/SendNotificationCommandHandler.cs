using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Commands.SendNotification;

public class SendNotificationCommandHandler(IApplicationDbContext db)
    : IRequestHandler<SendNotificationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(SendNotificationCommand request, CancellationToken ct)
    {
        db.NotificationQueues.Add(new NotificationQueue
        {
            Type = request.Type,
            Recipient = request.Recipient,
            Subject = request.Subject,
            Payload = request.Payload,
            Status = "Pending"
        });
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
