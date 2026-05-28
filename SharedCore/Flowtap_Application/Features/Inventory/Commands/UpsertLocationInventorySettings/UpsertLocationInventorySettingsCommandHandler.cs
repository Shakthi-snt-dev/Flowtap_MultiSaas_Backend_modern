using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpsertLocationInventorySettings;

public class UpsertLocationInventorySettingsCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpsertLocationInventorySettingsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpsertLocationInventorySettingsCommand request, CancellationToken ct)
    {
        var settings = await db.LocationInventorySettings
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.LocationId == request.LocationId, ct);

        if (settings is null)
        {
            settings = new LocationInventorySettings
            {
                CompanyId = request.CompanyId,
                LocationId = request.LocationId
            };
            db.LocationInventorySettings.Add(settings);
        }

        if (request.EnableBinTracking.HasValue)   settings.EnableBinTracking = request.EnableBinTracking.Value;
        if (request.AllowNegativeStock.HasValue)  settings.AllowNegativeStock = request.AllowNegativeStock.Value;
        if (request.EnableAutoReorder.HasValue)   settings.EnableAutoReorder = request.EnableAutoReorder.Value;
        if (request.ReorderNotificationEmail is not null) settings.ReorderNotificationEmail = request.ReorderNotificationEmail;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
