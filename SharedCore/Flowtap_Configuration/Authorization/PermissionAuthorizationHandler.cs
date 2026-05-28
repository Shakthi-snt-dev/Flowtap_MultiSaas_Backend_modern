using Microsoft.AspNetCore.Authorization;

namespace Flowtap_Presentation.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Owner tokens have no "isEmployee" claim → full access, always pass.
        if (!context.User.HasClaim("isEmployee", "true"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Employee tokens must carry a "permission" claim matching the required module.
        if (context.User.HasClaim("permission", requirement.Module))
            context.Succeed(requirement);

        // If not succeeded → 403 Forbidden is returned automatically.
        return Task.CompletedTask;
    }
}
