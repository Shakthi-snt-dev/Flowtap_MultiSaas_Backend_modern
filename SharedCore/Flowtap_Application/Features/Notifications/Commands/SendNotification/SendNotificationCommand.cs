using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Commands.SendNotification;

public record SendNotificationCommand(
    string Type, string Recipient, string Subject, string Payload) : IRequest<Result<bool>>;
