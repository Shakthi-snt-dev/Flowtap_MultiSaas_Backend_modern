using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class CampaignProductRule : BaseEntity
{
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public Guid? ProductId { get; set; }
    public Guid? CategoryId { get; set; }
    public string RuleType { get; set; } = string.Empty;
}
