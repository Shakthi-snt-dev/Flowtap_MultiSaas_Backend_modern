using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Queries.GetUnreadMessageCount;

public record GetUnreadMessageCountQuery(Guid CompanyId, Guid UserId) : IRequest<Result<int>>;
