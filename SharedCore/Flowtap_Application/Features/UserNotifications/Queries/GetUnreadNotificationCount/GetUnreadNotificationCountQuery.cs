using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.UserNotifications.Queries.GetUnreadNotificationCount;

public record GetUnreadNotificationCountQuery(Guid CompanyId, Guid UserId) : IRequest<Result<int>>;
