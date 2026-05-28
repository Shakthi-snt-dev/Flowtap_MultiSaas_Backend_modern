using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetLocationInventorySettings;

public class GetLocationInventorySettingsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetLocationInventorySettingsQuery, Result<LocationInventorySettingsDto?>>
{
    public async Task<Result<LocationInventorySettingsDto?>> Handle(GetLocationInventorySettingsQuery request, CancellationToken ct)
    {
        var settings = await db.LocationInventorySettings
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.LocationId == request.LocationId, ct);

        if (settings is null)
            return Result<LocationInventorySettingsDto?>.Success(null);

        return Result<LocationInventorySettingsDto?>.Success(new LocationInventorySettingsDto(
            settings.Id, settings.LocationId,
            settings.EnableBinTracking, settings.AllowNegativeStock,
            settings.EnableAutoReorder, settings.ReorderNotificationEmail));
    }
}
