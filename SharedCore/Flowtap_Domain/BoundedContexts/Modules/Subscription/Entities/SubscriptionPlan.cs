using Flowtap_Domain.BoundedContexts.Modules.Subscription.Enums;
using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;

public class SubscriptionPlan : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public BillingCycleType BillingCycle { get; set; }
    public decimal PricePerLocation { get; set; }
    public int BillingCycleInDays { get; set; }
    public decimal? YearlyDiscountPercent { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public string? FeaturesJson { get; set; }
    public int MaxLocations { get; set; } = 1;
    public int MaxEmployees { get; set; } = 5;
}
