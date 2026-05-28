using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Commands.MarkMessageAsRead;

public record MarkMessageAsReadCommand(Guid Id, Guid UserId) : IRequest<Result<bool>>;
