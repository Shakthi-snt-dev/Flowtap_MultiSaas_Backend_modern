using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Identity.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;
