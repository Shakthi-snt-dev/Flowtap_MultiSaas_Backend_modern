using Flowtap_Domain.BoundedContexts.Modules.Subscription.Enums;
using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;

public class CompanySubscription : TenantEntity
{
    public Guid SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime NextBillingDate { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public bool IsActive { get; set; } = true;
    public int TotalLocations { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<SubscriptionLocation> Locations { get; set; } = [];
    public ICollection<BillingInvoice> Invoices { get; set; } = [];
    public ICollection<SubscriptionChangeLog> ChangeLogs { get; set; } = [];
}
