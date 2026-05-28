using Flowtap_Domain.SharedKernel;

namespace Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;

public class BillingPayment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public BillingInvoice Invoice { get; set; } = null!;
    public decimal PaidAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
}
