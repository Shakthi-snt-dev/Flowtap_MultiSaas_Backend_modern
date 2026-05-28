using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Identity.Commands.ChangePassword;

/// <summary>
/// Changes or sets the password for the currently authenticated user.
/// - If the user has no password (HasPassword = false), CurrentPassword is not required.
/// - If the user already has a password, CurrentPassword must match before the new one is set.
/// </summary>
public record ChangePasswordCommand(
    string? CurrentPassword,   // null when user is setting a password for the first time
    string NewPassword
) : IRequest<Result<bool>>;
