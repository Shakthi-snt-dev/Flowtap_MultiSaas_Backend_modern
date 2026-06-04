using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Queries.GetServices;

public class GetServicesQueryHandler(IRepairDbContext db)
    : IRequestHandler<GetServicesQuery, Result<PaginatedList<ServiceDto>>>
{
    public async Task<Result<PaginatedList<ServiceDto>>> Handle(GetServicesQuery request, CancellationToken ct)
    {
        bool hasTierFilter = request.DeviceModelId.HasValue || request.ProductCategoryId.HasValue;

        var query = db.Services
            .Include(s => s.SupportedModels)
            .Where(s => s.CompanyId == request.CompanyId && s.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(s => s.Name.Contains(request.Search));

        // Fetch model-specific service IDs once (reused for both filtering and tier labelling)
        List<Guid> mappedServiceIds = request.DeviceModelId.HasValue
            ? await db.ServiceDeviceModelMappings
                .Where(m => m.DeviceModelId == request.DeviceModelId.Value)
                .Select(m => m.ServiceId)
                .ToListAsync(ct)
            : [];

        // Tier filtering: when DeviceModelId or ProductCategoryId supplied, return only relevant tiers
        if (hasTierFilter)
        {
            var categoryId = request.ProductCategoryId;
            var modelId    = request.DeviceModelId;

            query = query.Where(s =>
                // Tier 1: universal
                s.IsUniversal
                // Tier 2: category-specific, no model mappings
                || (categoryId.HasValue && s.ProductCategoryId == categoryId.Value && !s.SupportedModels.Any())
                // Tier 3: mapped to specific model
                || (modelId.HasValue && mappedServiceIds.Contains(s.Id)));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(s => s.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var dtos = items.Select(s =>
        {
            string? tier = null;
            if (hasTierFilter)
            {
                if (request.DeviceModelId.HasValue && mappedServiceIds.Contains(s.Id))
                    tier = "ModelSpecific";
                else if (request.ProductCategoryId.HasValue && s.ProductCategoryId == request.ProductCategoryId.Value)
                    tier = "Category";
                else if (s.IsUniversal)
                    tier = "Universal";
            }
            return new ServiceDto(
                s.Id, s.CompanyId, s.Name, s.Description,
                s.BasePrice, s.IsActive, s.IsUniversal, s.ServiceCategoryId, tier);
        }).ToList();

        return Result<PaginatedList<ServiceDto>>.Success(
            new PaginatedList<ServiceDto>(dtos, total, request.Page, request.PageSize));
    }
}

