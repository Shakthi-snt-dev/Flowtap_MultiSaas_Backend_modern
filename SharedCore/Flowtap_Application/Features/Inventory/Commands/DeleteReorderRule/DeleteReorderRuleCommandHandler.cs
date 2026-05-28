using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteReorderRule;

public class DeleteReorderRuleCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteReorderRuleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteReorderRuleCommand request, CancellationToken ct)
    {
        var rule = await db.ReorderRules
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.ReorderRule), request.Id);

        db.ReorderRules.Remove(rule);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
