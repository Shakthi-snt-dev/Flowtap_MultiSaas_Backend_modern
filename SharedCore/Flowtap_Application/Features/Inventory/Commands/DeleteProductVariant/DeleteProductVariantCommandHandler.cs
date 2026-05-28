using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteProductVariant;

public class DeleteProductVariantCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteProductVariantCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductVariantCommand request, CancellationToken ct)
    {
        var variant = await db.ProductVariants
            .FirstOrDefaultAsync(v => v.Id == request.Id && v.ProductId == request.ProductId, ct);
        if (variant is null)
            return Result<bool>.Failure("Variant not found.");

        variant.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
