using Flowtap_Application.Common.Interfaces;

namespace Flowtap_Medical.Industry;

public class MedicalPermissionModule : IIndustryPermissionModule
{
    public IReadOnlyList<string> PermissionModules { get; } =
    [
        "Medical"
    ];
}
