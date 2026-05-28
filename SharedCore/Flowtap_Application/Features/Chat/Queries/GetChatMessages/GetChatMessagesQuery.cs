using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Chat.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Chat.Queries.GetChatMessages;

public record GetChatMessagesQuery(
    Guid ConversationId,
    Guid UserId,
    int Page = 1,
    int PageSize = 50,
    bool IsAdmin = false          // when true, bypasses participant check
) : IRequest<Result<List<ChatMessageDto>>>;
