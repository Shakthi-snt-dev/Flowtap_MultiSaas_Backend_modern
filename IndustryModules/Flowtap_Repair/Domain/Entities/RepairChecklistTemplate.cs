using Flowtap_Domain.SharedKernel;
namespace Flowtap_Repair.Domain.Entities;
public class RepairChecklistTemplate : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string JsonItems { get; set; } = "[]";
    public bool IsActive { get; set; } = true;
}
