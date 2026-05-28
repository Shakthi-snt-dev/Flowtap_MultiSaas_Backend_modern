using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateInventorySettings;

public class UpdateInventorySettingsCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateInventorySettingsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateInventorySettingsCommand request, CancellationToken ct)
    {
        var settings = await db.InventorySettings
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId, ct);

        if (settings is null)
        {
            settings = new InventorySettings { CompanyId = request.CompanyId };
            db.InventorySettings.Add(settings);
        }

        if (request.DefaultValuationMethod is not null &&
            Enum.TryParse<ValuationMethod>(request.DefaultValuationMethod, true, out var vm))
            settings.DefaultValuationMethod = vm;

        if (request.StockDeductionMode is not null &&
            Enum.TryParse<StockDeductionMode>(request.StockDeductionMode, true, out var sdm))
            settings.StockDeductionMode = sdm;

        if (request.NegativeStockPolicy is not null &&
            Enum.TryParse<NegativeStockPolicy>(request.NegativeStockPolicy, true, out var nsp))
            settings.NegativeStockPolicy = nsp;

        if (request.EnableBinTracking.HasValue)                   settings.EnableBinTracking = request.EnableBinTracking.Value;
        if (request.EnableSerialTracking.HasValue)                settings.EnableSerialTracking = request.EnableSerialTracking.Value;
        if (request.EnableAutoReorder.HasValue)                   settings.EnableAutoReorder = request.EnableAutoReorder.Value;
        if (request.LowStockNotificationEnabled.HasValue)         settings.LowStockNotificationEnabled = request.LowStockNotificationEnabled.Value;
        if (request.AllowBackDatingTransactions.HasValue)         settings.AllowBackDatingTransactions = request.AllowBackDatingTransactions.Value;
        if (request.RequireManagerApprovalForWriteOff.HasValue)   settings.RequireManagerApprovalForWriteOff = request.RequireManagerApprovalForWriteOff.Value;
        if (request.RequireSerialOnSale.HasValue)                 settings.RequireSerialOnSale = request.RequireSerialOnSale.Value;
        if (request.EnableBatchTracking.HasValue)                 settings.EnableBatchTracking = request.EnableBatchTracking.Value;
        if (request.AutoGenerateSku.HasValue)                     settings.AutoGenerateSku = request.AutoGenerateSku.Value;
        if (request.DeadStockDaysThreshold.HasValue)              settings.DeadStockDaysThreshold = request.DeadStockDaysThreshold.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
