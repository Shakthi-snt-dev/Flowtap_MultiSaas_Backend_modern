using Flowtap_Application.Common.DTOs;
using MediatR;
using Flowtap_Application.Features.Identity.DTOs;

namespace Flowtap_Application.Features.Identity.Commands.Login;

public record LoginCommand(string Email, string Password, string? DeviceInfo, string? IpAddress) : IRequest<Result<AuthResponseDto>>;
