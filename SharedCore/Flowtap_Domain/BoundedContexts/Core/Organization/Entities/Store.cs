using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
public class Store : AuditableEntity
{
    public Guid CompanyId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid DefaultOrderType { get; set; }
    public int CurrentOrderCounter { get; set; }
    public Guid? DefaultCashierId { get; set; }
    public bool AlwaysUseDefaultCashier { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string TimeZoneId { get; set; } = "UTC";
    public string? LocationCode { get; set; }
    public Guid? TaxConfigurationId { get; set; }
    public TaxConfiguration? TaxConfiguration { get; set; }
    public bool IsActive { get; set; } = true;
    // Food industry — default kitchen warehouse for this branch/franchise location
    public Guid? DefaultWarehouseId { get; set; }
    public ICollection<LocationOrderType> SupportedOrderTypes { get; set; } = [];
}
