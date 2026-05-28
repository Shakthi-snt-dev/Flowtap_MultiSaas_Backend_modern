using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Notifications.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Queries.GetAdminBroadcasts;

public record GetAdminBroadcastsQuery(
    Guid CompanyId,
    Guid UserId,
    Guid? LocationId = null,
    int Limit = 20,
    string? Type = null,       // "Banner" | "Popup" | "Notification" | null = all
    bool ActiveOnly = true
) : IRequest<Result<List<AdminBroadcastDto>>>;
