using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class Client : TenantEntity
{
    public Guid LocationId { get; set; }
    public ClientType Type { get; set; } = ClientType.Individual;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    public string? CompanyName { get; set; }
    public string? GSTIN { get; set; }
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? ReferralSource { get; set; }
    public string? Notes { get; set; }
    public decimal DiscountPercent { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public new bool IsActive { get; set; } = true;
    public Guid? CreatedByEmployeeId { get; set; }
    public Guid? UpdatedByEmployeeId { get; set; }
    public ICollection<Sale> Sales { get; set; } = [];
}

