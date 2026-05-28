using Flowtap_Domain.SharedKernel;

namespace Flowtap_Food.Domain.Entities;

/// <summary>
/// Per-product low-stock alert rule for the food industry kitchen pantry.
/// When WarehouseStock.Quantity falls below Threshold, notifications are sent.
/// </summary>
public class StockAlertRule : TenantEntity
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal Threshold { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool SendEmail { get; set; }
    public bool SendSms { get; set; }
    public bool SendWhatsApp { get; set; }
    public string? RecipientContact { get; set; }
    public DateTime? LastTriggeredAt { get; set; }
}
