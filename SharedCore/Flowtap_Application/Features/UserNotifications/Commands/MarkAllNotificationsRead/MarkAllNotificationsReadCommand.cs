using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.UserNotifications.Commands.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand(Guid CompanyId, Guid UserId) : IRequest<Result<bool>>;
