using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;

public class BillingInvoice : AuditableEntity
{
    public Guid CompanySubscriptionId { get; set; }
    public CompanySubscription CompanySubscription { get; set; } = null!;
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsPaid { get; set; }
    public ICollection<BillingPayment> Payments { get; set; } = [];
}
