using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Flowtap_Domain.SharedKernel;
namespace Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
public class InventorySettings : TenantEntity
{
    public ValuationMethod DefaultValuationMethod { get; set; } = ValuationMethod.FIFO;
    public StockDeductionMode StockDeductionMode { get; set; } = StockDeductionMode.OnTicketClose;
    public NegativeStockPolicy NegativeStockPolicy { get; set; } = NegativeStockPolicy.Warn;
    public bool EnableBinTracking { get; set; }
    public bool EnableSerialTracking { get; set; }
    public bool EnableAutoReorder { get; set; }
    public bool LowStockNotificationEnabled { get; set; } = true;
    public bool AllowBackDatingTransactions { get; set; }
    public bool RequireManagerApprovalForWriteOff { get; set; } = true;
    public bool RequireSerialOnSale { get; set; }
    public bool EnableBatchTracking { get; set; }
    public bool AutoGenerateSku { get; set; } = true;
    public Guid? DefaultReorderSupplierId { get; set; }
    public int DeadStockDaysThreshold { get; set; } = 90;
}
