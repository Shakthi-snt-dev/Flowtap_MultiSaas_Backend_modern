using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class PaymentGatewayTransaction : TenantEntity
{
    public GatewayProvider Gateway { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ExternalOrderId { get; set; } = string.Empty;
    public string? ExternalPaymentId { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid? SaleId { get; set; }
    public Guid? TicketId { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string? FailureReason { get; set; }
}
