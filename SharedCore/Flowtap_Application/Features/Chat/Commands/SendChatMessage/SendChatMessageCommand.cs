using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Chat.Commands.SendChatMessage;

public record SendChatMessageCommand(
    Guid ConversationId,
    Guid SenderId,
    string SenderName,
    string Body,
    Guid CompanyId
) : IRequest<Result<Guid>>;
