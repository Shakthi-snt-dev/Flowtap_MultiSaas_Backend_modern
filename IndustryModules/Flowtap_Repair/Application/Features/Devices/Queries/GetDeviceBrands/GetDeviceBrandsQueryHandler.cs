using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.Application.Features.Devices.Commands.CreateDeviceBrand;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Queries.GetDeviceBrands;

public class GetDeviceBrandsQueryHandler(Flowtap_Repair.DbContext.IRepairDbContext context)
    : IRequestHandler<GetDeviceBrandsQuery, Result<List<DeviceBrandDto>>>
{
    public async Task<Result<List<DeviceBrandDto>>> Handle(GetDeviceBrandsQuery request, CancellationToken ct)
    {
        var query = context.DeviceBrands.AsNoTracking();

        if (request.ProductCategoryId.HasValue)
            query = query.Where(b => b.ProductCategoryId == request.ProductCategoryId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(b => b.Name.Contains(request.SearchTerm));

        if (request.IsActive.HasValue)
            query = query.Where(b => b.IsActive == request.IsActive.Value);

        var brands = await query.ToListAsync(ct);

        var dtos = brands.Select(b => new DeviceBrandDto(
            b.Id,
            b.ProductCategoryId,
            b.Name,
            b.IconUrl,
            b.Color,
            b.IsActive)).ToList();

        return Result<List<DeviceBrandDto>>.Success(dtos);
    }
}

