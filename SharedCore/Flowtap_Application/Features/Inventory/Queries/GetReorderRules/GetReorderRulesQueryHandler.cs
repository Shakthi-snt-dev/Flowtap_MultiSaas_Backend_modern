using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetReorderRules;

public class GetReorderRulesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetReorderRulesQuery, Result<List<ReorderRuleDto>>>
{
    public async Task<Result<List<ReorderRuleDto>>> Handle(GetReorderRulesQuery request, CancellationToken ct)
    {
        var rules = await db.ReorderRules
            .Include(r => r.Product)
            .Include(r => r.Warehouse)
            .Where(r => r.CompanyId == request.CompanyId)
            .OrderBy(r => r.Product.Name)
            .ToListAsync(ct);

        var dtos = rules.Select(r => new ReorderRuleDto(
            r.Id,
            r.ProductId,
            r.Product.Name,
            r.WarehouseId,
            r.Warehouse.Name,
            r.MinimumQuantity,
            r.ReorderQuantity,
            r.LeadTimeDays,
            r.IsActive
        )).ToList();

        return Result<List<ReorderRuleDto>>.Success(dtos);
    }
}
