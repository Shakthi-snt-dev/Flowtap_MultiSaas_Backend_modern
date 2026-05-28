using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Purchase.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Purchase.Queries.GetSuppliers;

public class GetSuppliersQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetSuppliersQuery, Result<PaginatedList<SupplierDto>>>
{
    public async Task<Result<PaginatedList<SupplierDto>>> Handle(GetSuppliersQuery request, CancellationToken ct)
    {
        var query = db.Suppliers.Where(s => s.CompanyId == request.CompanyId);

        if (request.ActiveOnly) query = query.Where(s => s.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(s => s.Name.Contains(request.Search)
                || (s.Phone != null && s.Phone.Contains(request.Search))
                || (s.Email != null && s.Email.Contains(request.Search)));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(s => s.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return Result<PaginatedList<SupplierDto>>.Success(
            new PaginatedList<SupplierDto>(mapper.Map<List<SupplierDto>>(items), total, request.Page, request.PageSize));
    }
}
