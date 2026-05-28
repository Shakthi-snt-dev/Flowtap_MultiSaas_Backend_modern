using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Organization.StoreSetting.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.StoreSetting.Queries.GetStoreSetting;

public class GetStoreSettingQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetStoreSettingQuery, Result<StoreSettingDto>>
{
    public async Task<Result<StoreSettingDto>> Handle(GetStoreSettingQuery request, CancellationToken ct)
    {
        var s = await db.StoreSettings
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId && x.LocationId == request.LocationId, ct);

        // Return defaults if not yet saved for this store
        if (s is null)
        {
            return Result<StoreSettingDto>.Success(new StoreSettingDto(
                Guid.Empty, request.CompanyId, request.LocationId,
                "light", "default", "blue", "inter", "normal", "comfortable",
                false, true, 100, true, false, false,
                null, null, null));
        }

        return Result<StoreSettingDto>.Success(new StoreSettingDto(
            s.Id, s.CompanyId, s.LocationId,
            s.ThemeMode, s.ColorTheme, s.AccentColor, s.FontFamily, s.BorderRadius, s.SidebarDensity,
            s.RequireClientOnSale, s.AllowDiscount, s.MaxDiscountPercent,
            s.AllowVoid, s.RequireManagerPinForVoid, s.AutoPrintReceipt,
            s.ReceiptFooterText, s.OpeningTime, s.ClosingTime));
    }
}
