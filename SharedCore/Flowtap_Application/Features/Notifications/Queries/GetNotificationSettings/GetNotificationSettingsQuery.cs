using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Notifications.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Queries.GetNotificationSettings;

public record GetNotificationSettingsQuery(Guid UserAccountId) : IRequest<Result<NotificationSettingsDto>>;
