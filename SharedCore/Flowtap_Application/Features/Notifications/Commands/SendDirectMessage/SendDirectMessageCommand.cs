using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Commands.SendDirectMessage;

public record SendDirectMessageCommand(
    Guid   CompanyId,
    Guid   SenderId,
    Guid   RecipientId,
    string Subject,
    string Body,
    bool   IsComplaint = false
) : IRequest<Result<Guid>>;
