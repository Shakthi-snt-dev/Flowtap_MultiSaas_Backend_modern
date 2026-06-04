using Flowtap_Application.Common.Interfaces;

namespace Flowtap_Repair.Domain.Industry;

public class RepairPermissionModule : IIndustryPermissionModule
{
    public IReadOnlyList<string> PermissionModules { get; } =
    [
        "ServiceTickets"
    ];
}
