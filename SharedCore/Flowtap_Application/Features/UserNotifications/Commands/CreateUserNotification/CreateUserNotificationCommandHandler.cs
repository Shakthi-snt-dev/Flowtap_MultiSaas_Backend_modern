using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using MediatR;

namespace Flowtap_Application.Features.UserNotifications.Commands.CreateUserNotification;

public class CreateUserNotificationCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateUserNotificationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserNotificationCommand request, CancellationToken ct)
    {
        var notification = new UserNotification
        {
            CompanyId     = request.CompanyId,
            UserId        = request.UserId,
            Type          = request.Type,
            Title         = request.Title,
            Message       = request.Message,
            ReferenceId   = request.ReferenceId,
            ReferenceType = request.ReferenceType,
            IsRead        = false,
            CreatedAt     = DateTime.UtcNow,
        };
        db.UserNotifications.Add(notification);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(notification.Id);
    }
}
