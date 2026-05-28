using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Identity.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string OTP, string NewPassword) : IRequest<Result<bool>>;
