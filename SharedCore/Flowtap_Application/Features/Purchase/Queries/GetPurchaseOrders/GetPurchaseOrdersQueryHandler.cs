using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Purchase.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Purchase.Queries.GetPurchaseOrders;

public class GetPurchaseOrdersQueryHandler(IApplicationDbContext db, IMapper mapper, ICurrentUserService currentUser)
    : IRequestHandler<GetPurchaseOrdersQuery, Result<PaginatedList<PurchaseOrderDto>>>
{
    public async Task<Result<PaginatedList<PurchaseOrderDto>>> Handle(GetPurchaseOrdersQuery request, CancellationToken ct)
    {
        var query = db.PurchaseOrders
            .Include(p => p.Items)
            .Where(p => p.CompanyId == request.CompanyId);

        var storeId = currentUser.StoreId;
        if (storeId.HasValue && storeId.Value != Guid.Empty)
        {
            query = query.Where(p => p.LocationId == storeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<PurchaseOrderStatus>(request.Status, ignoreCase: true, out var statusEnum))
            query = query.Where(p => p.Status == statusEnum);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return Result<PaginatedList<PurchaseOrderDto>>.Success(
            new PaginatedList<PurchaseOrderDto>(mapper.Map<List<PurchaseOrderDto>>(items), total, request.Page, request.PageSize));
    }
}
