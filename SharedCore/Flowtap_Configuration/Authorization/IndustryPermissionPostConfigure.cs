using Flowtap_Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Flowtap_Presentation.Authorization;

/// <summary>
/// PostConfigure that runs after all services are registered and
/// adds permission policies for each IIndustryPermissionModule.
/// This runs lazily (when IOptions&lt;AuthorizationOptions&gt; is first resolved),
/// so it sees ALL IIndustryPermissionModule instances regardless of registration order.
/// </summary>
public class IndustryPermissionPostConfigure(
    IEnumerable<IIndustryPermissionModule> permissionModules)
    : IPostConfigureOptions<AuthorizationOptions>
{
    public void PostConfigure(string? name, AuthorizationOptions options)
    {
        foreach (var module in permissionModules.SelectMany(m => m.PermissionModules).Distinct())
        {
            var policyName = $"{RequirePermissionAttribute.PolicyPrefix}{module}";
            if (options.GetPolicy(policyName) is null)
            {
                // Capture loop variable explicitly to avoid closure issues
                var capturedModule = module;
                options.AddPolicy(policyName, policy => policy
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(capturedModule)));
            }
        }
    }
}
