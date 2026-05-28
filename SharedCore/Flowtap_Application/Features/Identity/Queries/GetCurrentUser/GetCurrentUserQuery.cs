using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Identity.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Identity.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<Result<UserDto>>;
