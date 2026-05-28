using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Notifications.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(
    Guid CompanyId,
    string? Channel = null,   // "Email" | "Sms"
    string? Status = null,    // "Pending" | "Sent" | "Failed"
    int Page = 1,
    int PageSize = 30
) : IRequest<Result<PaginatedList<NotificationListItemDto>>>;
