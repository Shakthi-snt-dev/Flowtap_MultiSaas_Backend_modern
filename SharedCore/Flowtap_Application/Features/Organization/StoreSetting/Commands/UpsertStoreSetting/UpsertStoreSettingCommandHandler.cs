using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.StoreSetting.Commands.UpsertStoreSetting;

public class UpsertStoreSettingCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpsertStoreSettingCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpsertStoreSettingCommand request, CancellationToken ct)
    {
        var existing = await db.StoreSettings
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId && x.LocationId == request.LocationId, ct);

        if (existing is null)
        {
            existing = new Flowtap_Domain.BoundedContexts.Core.Organization.Entities.StoreSetting
            {
                CompanyId  = request.CompanyId,
                LocationId = request.LocationId,
            };
            db.StoreSettings.Add(existing);
        }

        // Patch only provided fields
        if (request.ThemeMode    is not null) existing.ThemeMode    = request.ThemeMode;
        if (request.ColorTheme   is not null) existing.ColorTheme   = request.ColorTheme;
        if (request.AccentColor  is not null) existing.AccentColor  = request.AccentColor;
        if (request.FontFamily   is not null) existing.FontFamily   = request.FontFamily;
        if (request.BorderRadius is not null) existing.BorderRadius = request.BorderRadius;
        if (request.SidebarDensity is not null) existing.SidebarDensity = request.SidebarDensity;

        if (request.RequireClientOnSale   is not null) existing.RequireClientOnSale   = request.RequireClientOnSale.Value;
        if (request.AllowDiscount         is not null) existing.AllowDiscount         = request.AllowDiscount.Value;
        if (request.MaxDiscountPercent    is not null) existing.MaxDiscountPercent    = request.MaxDiscountPercent.Value;
        if (request.AllowVoid             is not null) existing.AllowVoid             = request.AllowVoid.Value;
        if (request.RequireManagerPinForVoid is not null) existing.RequireManagerPinForVoid = request.RequireManagerPinForVoid.Value;
        if (request.AutoPrintReceipt      is not null) existing.AutoPrintReceipt      = request.AutoPrintReceipt.Value;
        if (request.ReceiptFooterText     is not null) existing.ReceiptFooterText     = request.ReceiptFooterText;
        if (request.OpeningTime           is not null) existing.OpeningTime           = request.OpeningTime;
        if (request.ClosingTime           is not null) existing.ClosingTime           = request.ClosingTime;

        existing.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
