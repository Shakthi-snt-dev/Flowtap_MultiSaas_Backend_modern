using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class Campaign : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public CampaignType Type { get; set; }
    public decimal DiscountValue { get; set; }
    public string DiscountType { get; set; } = "Percent"; // Percent or Fixed
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CampaignStatus Status { get; set; } = CampaignStatus.Scheduled;
    public string TargetType { get; set; } = "All";
    public decimal BudgetAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<CampaignProductRule> Rules { get; set; } = [];
}
