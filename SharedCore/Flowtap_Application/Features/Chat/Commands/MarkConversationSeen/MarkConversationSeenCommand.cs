using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Chat.Commands.MarkConversationSeen;

public record MarkConversationSeenCommand(
    Guid ConversationId,
    Guid UserId
) : IRequest<Result<bool>>;
