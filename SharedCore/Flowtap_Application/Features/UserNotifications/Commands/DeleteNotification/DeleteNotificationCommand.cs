using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.UserNotifications.Commands.DeleteNotification;

public record DeleteNotificationCommand(Guid Id, Guid UserId) : IRequest<Result<bool>>;
