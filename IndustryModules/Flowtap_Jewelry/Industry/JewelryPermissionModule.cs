using Flowtap_Application.Common.Interfaces;

namespace Flowtap_Jewelry.Industry;

public class JewelryPermissionModule : IIndustryPermissionModule
{
    public IReadOnlyList<string> PermissionModules { get; } =
    [
        "Jewelry"
    ];
}
