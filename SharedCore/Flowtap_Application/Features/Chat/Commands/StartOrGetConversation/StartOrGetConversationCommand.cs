using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Chat.Commands.StartOrGetConversation;

public record StartOrGetConversationCommand(
    Guid CompanyId,
    Guid CurrentUserId,
    string CurrentUserName,
    Guid OtherUserId,
    string OtherUserName,
    Guid? LocationId = null  // store context where the chat was initiated
) : IRequest<Result<Guid>>;
