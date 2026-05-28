using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.UserNotifications.DTOs;
using MediatR;

namespace Flowtap_Application.Features.UserNotifications.Queries.GetUserNotifications;

public record GetUserNotificationsQuery(
    Guid CompanyId,
    Guid UserId,
    bool? IsRead = null,
    int Page = 1,
    int PageSize = 30
) : IRequest<Result<List<UserNotificationDto>>>;
