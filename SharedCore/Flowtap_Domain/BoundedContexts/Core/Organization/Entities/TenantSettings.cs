using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class TenantSettings : BaseEntity
{
    public Guid CompanyId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public int MaxLocations { get; set; } = 1;
    public int MaxEmployees { get; set; } = 5;
    public string? AllowedModules { get; set; }
    public bool IsOnboardingComplete { get; set; }
    public string? TimeZoneId { get; set; }
    public string? LogoUrl { get; set; }
}
