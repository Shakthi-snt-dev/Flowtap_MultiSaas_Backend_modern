using Flowtap_Application.Common.Interfaces;

namespace Flowtap_Hotel.Industry;

public class HotelPermissionModule : IIndustryPermissionModule
{
    public IReadOnlyList<string> PermissionModules { get; } =
    [
        "Hotel"
    ];
}
