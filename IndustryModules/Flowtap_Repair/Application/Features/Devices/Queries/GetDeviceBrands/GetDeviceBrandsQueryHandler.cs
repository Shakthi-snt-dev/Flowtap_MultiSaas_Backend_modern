using Flowtap_Repair.Domain.Entities;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.Application.Features.Devices.Commands.CreateDeviceBrand;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Devices.Queries.GetDeviceBrands;

public class GetDeviceBrandsQueryHandler(
    Flowtap_Repair.DbContext.IRepairDbContext context,
    IApplicationDbContext coreDb)
    : IRequestHandler<GetDeviceBrandsQuery, Result<List<DeviceBrandDto>>>
{
    public async Task<Result<List<DeviceBrandDto>>> Handle(GetDeviceBrandsQuery request, CancellationToken ct)
    {
        var query = context.DeviceBrands.AsNoTracking();

        // Category filter with optional sub-category expansion.
        // Example: ProductCategoryId=Mobile + IncludeSubCategories=true
        //   → finds [Mobile, Android, iPhone] IDs and returns brands assigned to ANY of them.
        //   → Samsung (productCategoryId=Android) will now appear when "Mobile" is selected.
        if (request.ProductCategoryId.HasValue)
        {
            if (request.IncludeSubCategories)
            {
                var childIds = await coreDb.ProductCategories
                    .Where(c => c.ParentCategoryId == request.ProductCategoryId.Value && c.IsActive)
                    .Select(c => c.Id)
                    .ToListAsync(ct);
                var allCategoryIds = childIds.Append(request.ProductCategoryId.Value).ToList();
                query = query.Where(b => b.ProductCategoryId.HasValue
                                      && allCategoryIds.Contains(b.ProductCategoryId.Value));
            }
            else
            {
                query = query.Where(b => b.ProductCategoryId == request.ProductCategoryId.Value);
            }
        }

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

