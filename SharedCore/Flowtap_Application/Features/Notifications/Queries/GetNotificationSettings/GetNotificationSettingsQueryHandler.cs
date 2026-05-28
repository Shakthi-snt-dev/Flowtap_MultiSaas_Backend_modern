using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Notifications.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Notifications.Queries.GetNotificationSettings;

public class GetNotificationSettingsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetNotificationSettingsQuery, Result<NotificationSettingsDto>>
{
    public async Task<Result<NotificationSettingsDto>> Handle(GetNotificationSettingsQuery request, CancellationToken ct)
    {
        var settings = await db.UserNotificationSettings
            .FirstOrDefaultAsync(s => s.UserAccountId == request.UserAccountId, ct);

        if (settings is null)
        {
            // Return defaults if no settings saved yet
            return Result<NotificationSettingsDto>.Success(new NotificationSettingsDto(
                true, false, true, true, true, true, true));
        }

        return Result<NotificationSettingsDto>.Success(new NotificationSettingsDto(
            settings.AssignedToTicketEmail,
            settings.AssignedToTicketSms,
            settings.TicketStatusChangedPush,
            settings.TaskAssignedEmail,
            settings.TaskStatusChangedPush,
            settings.LowStockAlert,
            settings.PaymentReceivedEmail));
    }
}
