using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class Tenant : AuditableEntity
{
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? Requisites { get; set; }
    public string Country { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string SubDomain { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public IndustryType IndustryType { get; set; }
    public string? ActiveModules { get; set; }
    public bool IsActive { get; set; } = true;
    public TenantSettings? Settings { get; set; }
    public ICollection<Store> Locations { get; set; } = [];
    public ICollection<TaxConfiguration> TaxConfigurations { get; set; } = [];
}
