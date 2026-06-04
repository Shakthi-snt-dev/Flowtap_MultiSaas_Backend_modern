namespace Flowtap_Application.Common.Interfaces;

/// <summary>
/// Implemented by each IndustryModule to declare the permission module names it introduces.
/// Register as singleton in [Industry]ServiceExtensions so PresentationServiceExtensions
/// can discover and build authorization policies dynamically.
/// </summary>
public interface IIndustryPermissionModule
{
    /// <summary>
    /// Returns the permission module names this industry owns.
    /// These map to RequirePermission("ModuleName") attributes on controllers.
    /// </summary>
    IReadOnlyList<string> PermissionModules { get; }
}
