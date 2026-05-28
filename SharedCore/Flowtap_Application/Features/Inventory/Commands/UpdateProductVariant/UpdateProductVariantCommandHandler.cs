using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateProductVariant;

public class UpdateProductVariantCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateProductVariantCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateProductVariantCommand request, CancellationToken ct)
    {
        var variant = await db.ProductVariants
            .FirstOrDefaultAsync(v => v.Id == request.Id && v.ProductId == request.ProductId, ct);
        if (variant is null)
            return Result<bool>.Failure("Variant not found.");

        if (request.Name is not null)           variant.Name = request.Name;
        if (request.SKU is not null)            variant.SKU = request.SKU;
        if (request.AdditionalPrice.HasValue)   variant.AdditionalPrice = request.AdditionalPrice;
        if (request.IsActive.HasValue)          variant.IsActive = request.IsActive.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
