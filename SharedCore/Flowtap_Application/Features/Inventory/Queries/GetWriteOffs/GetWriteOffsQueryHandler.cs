using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetWriteOffs;

public class GetWriteOffsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetWriteOffsQuery, Result<PaginatedList<WriteOffDto>>>
{
    public async Task<Result<PaginatedList<WriteOffDto>>> Handle(GetWriteOffsQuery request, CancellationToken ct)
    {
        var query = db.InventoryWriteOffs
            .Include(w => w.Product)
            .Include(w => w.Warehouse)
            .Where(w => w.CompanyId == request.CompanyId);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<WriteOffStatus>(request.Status, true, out var statusEnum))
            query = query.Where(w => w.Status == statusEnum);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(w => w.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(w => new WriteOffDto(
                w.Id, w.ProductId,
                w.Product != null ? w.Product.Name : string.Empty,
                w.WarehouseId,
                w.Warehouse != null ? w.Warehouse.Name : string.Empty,
                w.WriteOffNumber, w.Quantity, w.Type.ToString(), w.Reason, w.Status.ToString(), w.CreatedAt))
            .ToListAsync(ct);

        return Result<PaginatedList<WriteOffDto>>.Success(
            new PaginatedList<WriteOffDto>(items, total, request.Page, request.PageSize));
    }
}
