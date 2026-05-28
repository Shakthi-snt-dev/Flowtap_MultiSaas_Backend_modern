using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteProductCategory;

public class DeleteProductCategoryCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteProductCategoryCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductCategoryCommand request, CancellationToken ct)
    {
        var category = await db.ProductCategories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.ProductCategory), request.Id);

        category.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
