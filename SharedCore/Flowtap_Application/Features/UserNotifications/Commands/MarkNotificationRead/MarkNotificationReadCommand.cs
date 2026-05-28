using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.UserNotifications.Commands.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid Id, Guid UserId) : IRequest<Result<bool>>;
