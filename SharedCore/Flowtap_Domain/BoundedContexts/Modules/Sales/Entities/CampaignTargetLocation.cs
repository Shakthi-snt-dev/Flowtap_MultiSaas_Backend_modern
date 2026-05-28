using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class CampaignTargetLocation : BaseEntity
{
    public Guid CampaignId { get; set; }
    public MarketingCampaign Campaign { get; set; } = null!;
    public Guid LocationId { get; set; }
}
