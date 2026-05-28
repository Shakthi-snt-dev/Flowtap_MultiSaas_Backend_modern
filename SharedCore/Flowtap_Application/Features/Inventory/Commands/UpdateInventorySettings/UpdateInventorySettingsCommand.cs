using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateInventorySettings;

public record UpdateInventorySettingsCommand(
    Guid CompanyId,
    string? DefaultValuationMethod = null,
    string? StockDeductionMode = null,
    string? NegativeStockPolicy = null,
    bool? EnableBinTracking = null,
    bool? EnableSerialTracking = null,
    bool? EnableAutoReorder = null,
    bool? LowStockNotificationEnabled = null,
    bool? AllowBackDatingTransactions = null,
    bool? RequireManagerApprovalForWriteOff = null,
    bool? RequireSerialOnSale = null,
    bool? EnableBatchTracking = null,
    bool? AutoGenerateSku = null,
    int? DeadStockDaysThreshold = null) : IRequest<Result<bool>>;
