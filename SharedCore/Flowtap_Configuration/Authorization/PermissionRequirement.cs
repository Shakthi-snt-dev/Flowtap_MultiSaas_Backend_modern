using Microsoft.AspNetCore.Authorization;

namespace Flowtap_Presentation.Authorization;

/// <summary>
/// Authorization requirement that checks whether the current user has access
/// to a specific module.  Owner JWTs (no "isEmployee" claim) always pass.
/// Employee JWTs must carry a matching "permission" claim embedded at PIN-login.
/// </summary>
public class PermissionRequirement(string module) : IAuthorizationRequirement
{
    public string Module { get; } = module;
}
