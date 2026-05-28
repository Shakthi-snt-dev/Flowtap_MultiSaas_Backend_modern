using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await db.Products
            .FirstOrDefaultAsync(p => p.CompanyId == request.CompanyId && p.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.Product), request.Id);

        if (request.Name is not null) product.Name = request.Name;
        if (request.Tag is not null) product.Tag = request.Tag;
        if (request.HsnCode is not null) product.HsnCode = request.HsnCode;
        if (request.DefaultCostPrice.HasValue) product.DefaultCostPrice = request.DefaultCostPrice.Value;
        if (request.DefaultSalePrice.HasValue) product.DefaultSalePrice = request.DefaultSalePrice.Value;
        if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;
        if (request.TaxSlabId.HasValue) product.TaxSlabId = request.TaxSlabId.Value == Guid.Empty ? null : request.TaxSlabId.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
