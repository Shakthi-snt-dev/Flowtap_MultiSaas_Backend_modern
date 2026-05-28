using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.UserNotifications.Commands.CreateUserNotification;

public record CreateUserNotificationCommand(
    Guid CompanyId,
    Guid UserId,
    string Type,
    string Title,
    string Message,
    Guid? ReferenceId = null,
    string? ReferenceType = null
) : IRequest<Result<Guid>>;
