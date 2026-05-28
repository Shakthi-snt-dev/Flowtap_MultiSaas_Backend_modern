using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateProductCategory;

public class UpdateProductCategoryCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateProductCategoryCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateProductCategoryCommand request, CancellationToken ct)
    {
        var category = await db.ProductCategories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.ProductCategory), request.Id);

        if (request.Name is not null)      category.Name      = request.Name;
        if (request.IconUrl is not null)   category.IconUrl   = request.IconUrl;
        if (request.Color is not null)     category.Color     = request.Color;
        if (request.SortOrder.HasValue)    category.SortOrder = request.SortOrder.Value;
        if (request.IsActive.HasValue)     category.IsActive  = request.IsActive.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
