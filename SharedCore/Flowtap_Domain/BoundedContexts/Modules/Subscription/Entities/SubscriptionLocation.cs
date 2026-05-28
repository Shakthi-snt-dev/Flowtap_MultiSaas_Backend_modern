using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;

public class SubscriptionLocation : BaseEntity
{
    public Guid CompanySubscriptionId { get; set; }
    public CompanySubscription CompanySubscription { get; set; } = null!;
    public Guid LocationId { get; set; }
    public decimal LocationPrice { get; set; }
}
