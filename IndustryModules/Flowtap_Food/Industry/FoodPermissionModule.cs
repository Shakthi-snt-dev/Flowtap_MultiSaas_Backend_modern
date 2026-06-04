using Flowtap_Application.Common.Interfaces;

namespace Flowtap_Food.Industry;

public class FoodPermissionModule : IIndustryPermissionModule
{
    public IReadOnlyList<string> PermissionModules { get; } =
    [
        "Food"
    ];
}
