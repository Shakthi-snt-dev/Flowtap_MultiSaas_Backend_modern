using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetTransfers;

public class GetTransfersQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetTransfersQuery, Result<PaginatedList<TransferDto>>>
{
    public async Task<Result<PaginatedList<TransferDto>>> Handle(GetTransfersQuery request, CancellationToken ct)
    {
        var query = db.InventoryTransfers
            .Include(t => t.FromWarehouse)
            .Include(t => t.ToWarehouse)
            .Where(t => t.CompanyId == request.CompanyId);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<TransferStatus>(request.Status, true, out var statusEnum))
            query = query.Where(t => t.Status == statusEnum);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TransferDto(
                t.Id, t.CompanyId, t.TransferNumber, t.Status.ToString(),
                t.FromWarehouseId, t.FromWarehouse != null ? t.FromWarehouse.Name : string.Empty,
                t.ToWarehouseId,   t.ToWarehouse   != null ? t.ToWarehouse.Name   : string.Empty,
                t.CreatedAt, t.ShippedAt))
            .ToListAsync(ct);

        return Result<PaginatedList<TransferDto>>.Success(
            new PaginatedList<TransferDto>(items, total, request.Page, request.PageSize));
    }
}
