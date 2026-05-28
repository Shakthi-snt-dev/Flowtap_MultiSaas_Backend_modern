using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateReorderRule;

public class UpdateReorderRuleCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateReorderRuleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateReorderRuleCommand request, CancellationToken ct)
    {
        var rule = await db.ReorderRules
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.ReorderRule), request.Id);

        if (request.MinimumQuantity.HasValue) rule.MinimumQuantity = request.MinimumQuantity.Value;
        if (request.ReorderQuantity.HasValue) rule.ReorderQuantity = request.ReorderQuantity.Value;
        if (request.LeadTimeDays.HasValue)    rule.LeadTimeDays    = request.LeadTimeDays.Value;
        if (request.IsActive.HasValue)        rule.IsActive         = request.IsActive.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
