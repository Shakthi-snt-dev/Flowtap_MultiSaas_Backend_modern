using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.CreateReorderRule;

public class CreateReorderRuleCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateReorderRuleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateReorderRuleCommand request, CancellationToken ct)
    {
        var existing = await db.ReorderRules
            .AnyAsync(r => r.CompanyId == request.CompanyId
                && r.WarehouseId == request.WarehouseId
                && r.ProductId == request.ProductId, ct);

        if (existing)
            return Result<Guid>.Failure("A reorder rule already exists for this product/warehouse combination.");

        var rule = new ReorderRule
        {
            CompanyId = request.CompanyId,
            WarehouseId = request.WarehouseId,
            ProductId = request.ProductId,
            MinimumQuantity = request.MinimumQuantity,
            ReorderQuantity = request.ReorderQuantity,
            PreferredSupplierId = request.PreferredSupplierId,
            LeadTimeDays = request.LeadTimeDays,
            IsActive = true
        };

        db.ReorderRules.Add(rule);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(rule.Id);
    }
}
