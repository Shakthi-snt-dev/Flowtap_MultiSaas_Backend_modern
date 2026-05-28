using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Chat.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Chat.Queries.GetConversations;

public record GetConversationsQuery(
    Guid CompanyId,
    Guid UserId,
    Guid? LocationId = null,
    bool ViewAll = false          // when true (admin/owner), returns all company conversations
) : IRequest<Result<List<ConversationSummaryDto>>>;
