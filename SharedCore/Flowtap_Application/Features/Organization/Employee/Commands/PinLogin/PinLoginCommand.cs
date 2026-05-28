using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Employee.Commands.PinLogin;

public record PinLoginResponseDto(
    string AccessToken,
    Guid EmployeeId,
    Guid UserAccountId,
    string Name,
    string? Email,
    string? JobTitle,
    string? AvatarInitials,
    Dictionary<string, bool> Permissions,
    /// <summary>
    /// True when the PIN belongs to an Owner or Admin account.
    /// The frontend should grant full access with no module restrictions.
    /// </summary>
    bool IsOwner,
    Guid? DefaultLocationId = null
);

/// <summary>
/// Authenticate an employee by PIN and issue a real JWT for their user account.
/// This gives the employee a full session — their name, permissions, and token
/// replace the current user's session for the duration of their shift.
/// </summary>
public record PinLoginCommand(
    Guid CompanyId,
    string Pin
) : IRequest<Result<PinLoginResponseDto>>;
