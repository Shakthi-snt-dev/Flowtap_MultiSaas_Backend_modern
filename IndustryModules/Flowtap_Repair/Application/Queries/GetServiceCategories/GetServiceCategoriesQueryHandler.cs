using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Application.Commands.CreateServiceCategory;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Queries.GetServiceCategories;

public class GetServiceCategoriesQueryHandler(IRepairDbContext context)
    : IRequestHandler<GetServiceCategoriesQuery, Result<List<ServiceCategoryDto>>>
{
    public async Task<Result<List<ServiceCategoryDto>>> Handle(GetServiceCategoriesQuery request, CancellationToken ct)
    {
        var query = context.ServiceCategories
            .AsNoTracking()
            .Where(c => c.CompanyId == request.CompanyId);

        if (request.ParentCategoryId.HasValue)
            query = query.Where(c => c.ParentCategoryId == request.ParentCategoryId.Value);

        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);

        var categories = await query.ToListAsync(ct);

        var dtos = categories.Select(c => new ServiceCategoryDto(
            c.Id,
            c.CompanyId,
            c.ParentCategoryId,
            c.Name,
            c.Description,
            c.IconUrl,
            c.SortOrder,
            c.IsActive)).ToList();

        return Result<List<ServiceCategoryDto>>.Success(dtos);
    }
}

