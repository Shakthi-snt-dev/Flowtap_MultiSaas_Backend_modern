using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.AddProductMedia;

public class AddProductMediaCommandHandler(IApplicationDbContext db)
    : IRequestHandler<AddProductMediaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddProductMediaCommand request, CancellationToken ct)
    {
        var productExists = await db.Products.AnyAsync(p => p.Id == request.ProductId, ct);
        if (!productExists)
            return Result<Guid>.Failure("Product not found.");

        // If marking as primary, clear existing primary
        if (request.IsPrimary)
        {
            var existingMedia = await db.ProductMedia
                .Where(m => m.ProductId == request.ProductId && m.IsPrimary)
                .ToListAsync(ct);
            foreach (var m in existingMedia)
                m.IsPrimary = false;
        }

        var media = new ProductMedia
        {
            ProductId = request.ProductId,
            Url = request.Url,
            IsPrimary = request.IsPrimary,
            SortOrder = request.SortOrder
        };

        db.ProductMedia.Add(media);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(media.Id);
    }
}
