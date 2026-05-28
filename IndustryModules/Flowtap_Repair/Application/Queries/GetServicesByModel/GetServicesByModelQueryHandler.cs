using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Queries.GetServicesByModel;

public class GetServicesByModelQueryHandler(IRepairDbContext db)
    : IRequestHandler<GetServicesByModelQuery, Result<List<ServiceDto>>>
{
    public async Task<Result<List<ServiceDto>>> Handle(GetServicesByModelQuery request, CancellationToken ct)
    {
        // Get service IDs mapped to this device model
        var mappedServiceIds = await db.ServiceDeviceModelMappings
            .Where(m => m.DeviceModelId == request.DeviceModelId)
            .Select(m => m.ServiceId)
            .ToListAsync(ct);

        // Return services that are either mapped to the model OR are universal, filtered by company + active
        var services = await db.Services
            .Where(s => s.CompanyId == request.CompanyId && s.IsActive
                && (s.IsUniversal || mappedServiceIds.Contains(s.Id)))
            .OrderBy(s => s.Name)
            .Select(s => new ServiceDto(
                s.Id, s.CompanyId, s.Name, s.Description,
                s.BasePrice, s.IsActive, s.IsUniversal, s.ServiceCategoryId))
            .ToListAsync(ct);

        return Result<List<ServiceDto>>.Success(services);
    }
}

