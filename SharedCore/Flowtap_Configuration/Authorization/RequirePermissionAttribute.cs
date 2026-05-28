using Microsoft.AspNetCore.Authorization;

namespace Flowtap_Presentation.Authorization;

/// <summary>
/// Restricts an endpoint to users who have the given module permission.
/// Owners (no "isEmployee" JWT claim) are always allowed through.
/// Employees must have the matching "permission:{module}" claim in their JWT.
///
/// Usage:
///   [RequirePermission("POS")]
///   public class SaleController ...
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "Permission:";

    public RequirePermissionAttribute(string module)
        : base($"{PolicyPrefix}{module}")
    {
        Module = module;
    }

    public string Module { get; }
}
