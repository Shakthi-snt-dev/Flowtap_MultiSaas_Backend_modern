using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class MarketingCampaign : TenantEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public Guid? TargetProductId { get; set; }
    public new bool IsActive { get; set; } = true;
    public ICollection<CampaignTargetLocation> TargetLocations { get; set; } = [];
}

