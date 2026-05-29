using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Notifications.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(
    Guid    CompanyId,
    string? Channel        = null,   // "Email" | "Sms" | "WhatsApp"
    string? Status         = null,   // "Pending" | "Sent" | "Failed"
    string? SubjectContains = null,  // e.g. "[Kitchen Alert]" to show only stock alerts
    int     Page           = 1,
    int     PageSize       = 30
) : IRequest<Result<PaginatedList<NotificationListItemDto>>>;
