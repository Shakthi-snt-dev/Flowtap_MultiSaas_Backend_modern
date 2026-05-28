using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Chat.Commands.DeleteChatMessage;

public record DeleteChatMessageCommand(
    Guid MessageId,
    Guid SenderId
) : IRequest<Result<bool>>;
