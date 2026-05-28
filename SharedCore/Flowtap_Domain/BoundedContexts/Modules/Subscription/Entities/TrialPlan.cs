using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;

public class TrialPlan : TenantEntity
{
    public int TrialDays { get; set; } = 30;
    public DateTime TrialStartDate { get; set; }
    public DateTime TrialEndDate { get; set; }
    public bool IsExpired { get; set; }
    public int LocationCount { get; set; } = 1;
    public bool IsConverted { get; set; }
    public Guid? ConvertedToSubscriptionId { get; set; }
}
