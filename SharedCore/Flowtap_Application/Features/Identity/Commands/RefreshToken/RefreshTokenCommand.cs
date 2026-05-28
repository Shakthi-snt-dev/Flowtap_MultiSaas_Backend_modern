using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Identity.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Identity.Commands.RefreshToken;

public record RefreshTokenCommand(string Token) : IRequest<Result<TokenDto>>;
