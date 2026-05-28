using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Commands.DeleteDirectMessage;

public record DeleteDirectMessageCommand(Guid Id, Guid UserId) : IRequest<Result<bool>>;
