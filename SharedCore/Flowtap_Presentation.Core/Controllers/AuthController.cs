using Flowtap_Application.Features.Identity.Commands.ChangePassword;
using Flowtap_Application.Features.Identity.Commands.ForgotPassword;
using Flowtap_Application.Features.Identity.Commands.Login;
using Flowtap_Application.Features.Identity.Commands.Logout;
using Flowtap_Application.Features.Identity.Commands.RefreshToken;
using Flowtap_Application.Features.Identity.Commands.Register;
using Flowtap_Application.Features.Identity.Commands.ResendVerification;
using Flowtap_Application.Features.Identity.Commands.ResetPassword;
using Flowtap_Application.Features.Identity.Commands.VerifyEmail;
using Flowtap_Application.Features.Identity.Queries.GetCurrentUser;
using Flowtap_Application.Features.Identity.Queries.GetUserSessions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Controllers;

[Route("api/v1/auth")]
public class AuthController(ISender sender) : ApiController(sender)
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
        => Created(await Sender.Send(command, ct));

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
        => Ok(await Sender.Send(command, ct));

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
        => Ok(await Sender.Send(command, ct));

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command, ct));

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command, ct));

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command, ct));

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command, ct));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command, ct));

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser([FromQuery] GetCurrentUserQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions([FromQuery] GetUserSessionsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [AllowAnonymous]
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationEmailCommand command, CancellationToken ct)
        => FromResult(await Sender.Send(command, ct));
}
