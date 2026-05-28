using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.CreateProductVariant;

public class CreateProductVariantCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateProductVariantCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductVariantCommand request, CancellationToken ct)
    {
        var productExists = await db.Products.AnyAsync(p => p.Id == request.ProductId, ct);
        if (!productExists)
            return Result<Guid>.Failure("Product not found.");

        var variant = new ProductVariant
        {
            ProductId = request.ProductId,
            Name = request.Name,
            SKU = request.SKU ?? string.Empty,
            AdditionalPrice = request.AdditionalPrice,
            IsActive = true
        };

        db.ProductVariants.Add(variant);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(variant.Id);
    }
}
