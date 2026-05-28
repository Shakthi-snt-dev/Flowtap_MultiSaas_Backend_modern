using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Queries.GetSales;

public class GetSalesQueryHandler(IApplicationDbContext db, IMapper mapper, ICurrentUserService currentUser)
    : IRequestHandler<GetSalesQuery, Result<PaginatedList<SaleDto>>>
{
    public async Task<Result<PaginatedList<SaleDto>>> Handle(GetSalesQuery request, CancellationToken ct)
    {
        var query = db.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .Where(s => s.CompanyId == request.CompanyId);

        var locationId = request.LocationId ?? currentUser.StoreId;
        if (locationId.HasValue && locationId.Value != Guid.Empty)
            query = query.Where(s => s.LocationId == locationId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<SaleStatus>(request.Status, ignoreCase: true, out var statusEnum))
            query = query.Where(s => s.Status == statusEnum);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return Result<PaginatedList<SaleDto>>.Success(
            new PaginatedList<SaleDto>(mapper.Map<List<SaleDto>>(items), total, request.Page, request.PageSize));
    }
}
