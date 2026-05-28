using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetReorderAlerts;

public class GetReorderAlertsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetReorderAlertsQuery, Result<List<ReorderAlertDto>>>
{
    public async Task<Result<List<ReorderAlertDto>>> Handle(GetReorderAlertsQuery request, CancellationToken ct)
    {
        var query = db.ReorderAlerts
            .Include(a => a.Product)
            .Where(a => a.CompanyId == request.CompanyId);

        if (request.UnhandledOnly)
            query = query.Where(a => !a.IsHandled);

        var items = await query.OrderByDescending(a => a.CreatedAt).ToListAsync(ct);

        var dtos = items.Select(a => new ReorderAlertDto(
            a.Id, a.ProductId, a.Product.Name, a.WarehouseId,
            a.CurrentQuantity, a.ReorderLevel, a.Severity.ToString(), a.IsHandled)).ToList();

        return Result<List<ReorderAlertDto>>.Success(dtos);
    }
}
