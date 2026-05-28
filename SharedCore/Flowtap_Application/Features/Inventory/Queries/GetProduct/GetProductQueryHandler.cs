using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Inventory.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Inventory.Queries.GetProduct;

public class GetProductQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetProductQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken ct)
    {
        var product = await db.Products
            .Include(p => p.Variants)
            .Include(p => p.Media)
            .FirstOrDefaultAsync(p => p.CompanyId == request.CompanyId && p.Id == request.ProductId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.Product), request.ProductId);

        return Result<ProductDto>.Success(mapper.Map<ProductDto>(product));
    }
}
