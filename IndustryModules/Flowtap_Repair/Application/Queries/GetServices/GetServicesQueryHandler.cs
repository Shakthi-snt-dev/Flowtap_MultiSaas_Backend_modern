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
        var query = db.Services.Where(s => s.CompanyId == request.CompanyId && s.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(s => s.Name.Contains(request.Search));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(s => s.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var dtos = items.Select(s => new ServiceDto(
            s.Id, s.CompanyId, s.Name, s.Description,
            s.BasePrice, s.IsActive, s.IsUniversal, s.ServiceCategoryId)).ToList();

        return Result<PaginatedList<ServiceDto>>.Success(
            new PaginatedList<ServiceDto>(dtos, total, request.Page, request.PageSize));
    }
}

