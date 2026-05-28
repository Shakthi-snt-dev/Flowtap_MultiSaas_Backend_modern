using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Commands.ArchiveProduct;

public class ArchiveProductCommandHandler(IApplicationDbContext db)
    : IRequestHandler<ArchiveProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ArchiveProductCommand request, CancellationToken ct)
    {
        var product = await db.Products
            .FirstOrDefaultAsync(p => p.CompanyId == request.CompanyId && p.Id == request.ProductId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.Product), request.ProductId);

        product.IsActive = false;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
