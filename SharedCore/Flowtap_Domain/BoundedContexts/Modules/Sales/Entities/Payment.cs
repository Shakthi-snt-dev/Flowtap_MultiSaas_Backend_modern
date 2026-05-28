using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class Payment : TenantEntity
{
    public Guid? SaleId { get; set; }
    public Sale? Sale { get; set; }
    public Guid? TicketId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentPurpose Purpose { get; set; }
    public Guid AccountId { get; set; }
    public PaymentAccount Account { get; set; } = null!;
    public string? ExternalReference { get; set; }
    public string? Comment { get; set; }
    public Guid? EmployeeId { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public string? IdempotencyKey { get; set; }
}
