using Flowtap_Domain.SharedKernel;
using Flowtap_Jewelry.Domain.Enums;

namespace Flowtap_Jewelry.Domain.Entities;

public class MetalExchangeTransaction : TenantEntity
{
    public Guid LocationId { get; set; }
    public Guid? ClientId { get; set; }
    public string? ClientName { get; set; }
    public ExchangeType ExchangeType { get; set; }
    public MetalType MetalType { get; set; }
    public Purity Purity { get; set; }
    public decimal WeightGrams { get; set; }
    public decimal PurityPercent { get; set; }
    public decimal NetWeightGrams { get; set; }
    public decimal RatePerGram { get; set; }
    public decimal TotalValue { get; set; }
    public string? Description { get; set; }
    public string? ReferenceNumber { get; set; }
    public Guid? SaleId { get; set; }
}
