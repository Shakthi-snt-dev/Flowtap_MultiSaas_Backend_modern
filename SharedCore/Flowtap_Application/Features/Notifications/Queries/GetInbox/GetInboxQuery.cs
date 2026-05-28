using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Notifications.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Queries.GetInbox;

public record GetInboxQuery(
    Guid  CompanyId,
    Guid  UserId,
    Guid? LocationId = null,
    bool? IsComplaint = null,
    int   Page        = 1,
    int   PageSize    = 20
) : IRequest<Result<List<DirectMessageDto>>>;
