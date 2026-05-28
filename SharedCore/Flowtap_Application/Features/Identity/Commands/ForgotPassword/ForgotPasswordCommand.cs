using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Identity.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result<bool>>;
