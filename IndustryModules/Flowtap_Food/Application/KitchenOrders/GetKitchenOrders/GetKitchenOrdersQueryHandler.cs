using Flowtap_Application.Common.DTOs;
using Flowtap_Food.DbContext;
using Flowtap_Food.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Food.Application.KitchenOrders.GetKitchenOrders;

public class GetKitchenOrdersQueryHandler(IFoodDbContext db)
    : IRequestHandler<GetKitchenOrdersQuery, Result<List<KitchenOrderDto>>>
{
    public async Task<Result<List<KitchenOrderDto>>> Handle(GetKitchenOrdersQuery request, CancellationToken ct)
    {
        var query = db.KitchenOrders
            .Include(k => k.Table)
            .Include(k => k.Items)
            .Where(k => k.CompanyId == request.CompanyId);

        if (request.LocationId.HasValue)
            query = query.Where(k => k.LocationId == request.LocationId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<KOTStatus>(request.Status, true, out var statusFilter))
            query = query.Where(k => k.Status == statusFilter);

        var orders = await query
            .OrderByDescending(k => k.CreatedAt)
            .Select(k => new KitchenOrderDto(
                k.Id,
                k.LocationId,
                k.SaleId,
                k.TableId,
                k.Table != null ? k.Table.Name : null,
                k.OrderType.ToString(),
                k.Status.ToString(),
                k.KotNumber,
                k.Notes,
                k.PreparedAt,
                k.ServedAt,
                k.CreatedAt,
                k.Items.Select(i => new KitchenOrderItemDto(
                    i.Id, i.ProductId, i.ProductName, i.Quantity, i.Notes)).ToList()))
            .ToListAsync(ct);

        return Result<List<KitchenOrderDto>>.Success(orders);
    }
}
