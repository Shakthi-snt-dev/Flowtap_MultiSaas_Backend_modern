using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
public class Sale : TenantEntity
{
    public Guid LocationId { get; set; }
    public Guid? ClientId { get; set; }                  // null = walk-in customer (food/retail)
    public Client? Client { get; set; }
    public string? TransactionNumber { get; set; }
    public SaleSource Source { get; set; } = SaleSource.POS;
    public Guid? TicketId { get; set; }
    // Food industry extensions — null for all other industries
    public Guid? TableId { get; set; }
    public FoodOrderType? FoodOrderType { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Draft;
    public string? CancellationReason { get; set; }
    public string? RefundReason { get; set; }
    public string? Notes { get; set; }
    public string? IdempotencyKey { get; set; }
    public Guid? CashierEmployeeId { get; set; }   // employee who processed the sale at POS
    public ICollection<SaleItem> Items { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
    public ICollection<SaleHistory> History { get; set; } = [];
}
