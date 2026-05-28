using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetInventorySettings;

public class GetInventorySettingsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetInventorySettingsQuery, Result<InventorySettingsDto?>>
{
    public async Task<Result<InventorySettingsDto?>> Handle(GetInventorySettingsQuery request, CancellationToken ct)
    {
        var settings = await db.InventorySettings
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId, ct);

        if (settings is null)
            return Result<InventorySettingsDto?>.Success(null);

        var dto = new InventorySettingsDto(
            settings.Id,
            settings.DefaultValuationMethod.ToString(),
            settings.StockDeductionMode.ToString(),
            settings.NegativeStockPolicy.ToString(),
            settings.EnableBinTracking,
            settings.EnableSerialTracking,
            settings.EnableAutoReorder,
            settings.LowStockNotificationEnabled,
            settings.AllowBackDatingTransactions,
            settings.RequireManagerApprovalForWriteOff,
            settings.RequireSerialOnSale,
            settings.EnableBatchTracking,
            settings.AutoGenerateSku,
            settings.DeadStockDaysThreshold);

        return Result<InventorySettingsDto?>.Success(dto);
    }
}
