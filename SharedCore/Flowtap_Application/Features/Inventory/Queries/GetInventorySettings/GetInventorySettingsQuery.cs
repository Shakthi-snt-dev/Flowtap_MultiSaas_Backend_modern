using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetInventorySettings;

public record InventorySettingsDto(
    Guid Id,
    string DefaultValuationMethod,
    string StockDeductionMode,
    string NegativeStockPolicy,
    bool EnableBinTracking,
    bool EnableSerialTracking,
    bool EnableAutoReorder,
    bool LowStockNotificationEnabled,
    bool AllowBackDatingTransactions,
    bool RequireManagerApprovalForWriteOff,
    bool RequireSerialOnSale,
    bool EnableBatchTracking,
    bool AutoGenerateSku,
    int DeadStockDaysThreshold);

public record GetInventorySettingsQuery(Guid CompanyId) : IRequest<Result<InventorySettingsDto?>>;
