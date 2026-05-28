using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Notifications.Commands.DismissAdminBroadcast;

public record DismissAdminBroadcastCommand(Guid Id, Guid CompanyId, Guid UserId) : IRequest<Result<bool>>;
