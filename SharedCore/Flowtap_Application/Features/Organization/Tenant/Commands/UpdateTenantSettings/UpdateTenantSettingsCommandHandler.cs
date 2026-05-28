using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.UpdateTenantSettings;

public class UpdateTenantSettingsCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateTenantSettingsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateTenantSettingsCommand request, CancellationToken ct)
    {
        var settings = await db.TenantSettings
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId, ct);

        if (settings is null)
        {
            settings = new TenantSettings
            {
                CompanyId = request.CompanyId,
            };
            db.TenantSettings.Add(settings);
        }

        if (request.MaxLocations is not null) settings.MaxLocations = request.MaxLocations.Value;
        if (request.MaxEmployees is not null) settings.MaxEmployees = request.MaxEmployees.Value;
        if (request.TimeZoneId   is not null) settings.TimeZoneId   = request.TimeZoneId;
        if (request.LogoUrl      is not null) settings.LogoUrl      = request.LogoUrl;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
