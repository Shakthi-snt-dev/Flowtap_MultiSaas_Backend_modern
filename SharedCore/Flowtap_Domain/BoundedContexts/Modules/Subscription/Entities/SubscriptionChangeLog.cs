using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;

public class SubscriptionChangeLog : BaseEntity
{
    public Guid CompanySubscriptionId { get; set; }
    public CompanySubscription CompanySubscription { get; set; } = null!;
    public string ChangeType { get; set; } = string.Empty;
    public int OldLocationCount { get; set; }
    public int NewLocationCount { get; set; }
    public decimal OldAmount { get; set; }
    public decimal NewAmount { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
