using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.Application.Features.Devices.Commands.CreateDeviceModel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Queries.GetDeviceModels;

public class GetDeviceModelsQueryHandler(Flowtap_Repair.DbContext.IRepairDbContext context)
    : IRequestHandler<GetDeviceModelsQuery, Result<List<DeviceModelDto>>>
{
    public async Task<Result<List<DeviceModelDto>>> Handle(GetDeviceModelsQuery request, CancellationToken ct)
    {
        var query = context.DeviceModels
            .AsNoTracking()
            .Include(m => m.Brand);

        IQueryable<Flowtap_Repair.Domain.Entities.DeviceModel> filtered = query;

        if (request.BrandId.HasValue)
            filtered = filtered.Where(m => m.BrandId == request.BrandId.Value);

        if (request.ProductCategoryId.HasValue)
            filtered = filtered.Where(m => m.ProductCategoryId == request.ProductCategoryId.Value);

        if (request.ParentModelId.HasValue)
            filtered = filtered.Where(m => m.ParentModelId == request.ParentModelId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(m => m.Name.Contains(request.SearchTerm));

        if (request.IsActive.HasValue)
            filtered = filtered.Where(m => m.IsActive == request.IsActive.Value);

        var models = await filtered.ToListAsync(ct);

        var dtos = models.Select(m => new DeviceModelDto(
            m.Id,
            m.BrandId,
            m.Brand.Name,
            m.ParentModelId,
            m.ProductCategoryId,
            m.Name,
            m.ImageUrl,
            m.IsActive)).ToList();

        return Result<List<DeviceModelDto>>.Success(dtos);
    }
}

