using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetInventorySerials;

public class GetInventorySerialsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetInventorySerialsQuery, Result<PaginatedList<InventorySerialDto>>>
{
    public async Task<Result<PaginatedList<InventorySerialDto>>> Handle(GetInventorySerialsQuery request, CancellationToken ct)
    {
        var query = db.InventorySerials
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .Where(s => s.CompanyId == request.CompanyId);

        if (request.ProductId.HasValue)   query = query.Where(s => s.ProductId == request.ProductId.Value);
        if (request.WarehouseId.HasValue) query = query.Where(s => s.WarehouseId == request.WarehouseId.Value);
        if (request.IsSold.HasValue)      query = query.Where(s => s.IsSold == request.IsSold.Value);
        if (!string.IsNullOrWhiteSpace(request.SerialNumber))
            query = query.Where(s => s.ManufacturerSerial == request.SerialNumber || s.CompanySerial == request.SerialNumber);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(s => s.IsActive).ThenBy(s => s.CompanySerial)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new InventorySerialDto(
                s.Id, s.ProductId,
                s.Product != null ? s.Product.Name : string.Empty,
                s.WarehouseId,
                s.Warehouse != null ? s.Warehouse.Name : string.Empty,
                s.ManufacturerSerial, s.CompanySerial, s.DisplayName,
                s.IsSold, s.IsReturned, s.IsActive,
                s.WarrantyStartDate, s.WarrantyEndDate))
            .ToListAsync(ct);

        return Result<PaginatedList<InventorySerialDto>>.Success(
            new PaginatedList<InventorySerialDto>(items, total, request.Page, request.PageSize));
    }
}
