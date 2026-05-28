using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.DeleteProductMedia;

public class DeleteProductMediaCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteProductMediaCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductMediaCommand request, CancellationToken ct)
    {
        var media = await db.ProductMedia
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.ProductId == request.ProductId, ct);
        if (media is null)
            return Result<bool>.Failure("Media not found.");

        db.ProductMedia.Remove(media);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
