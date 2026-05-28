using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Queries.GetSale;

public class GetSaleQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetSaleQuery, Result<SaleDto>>
{
    public async Task<Result<SaleDto>> Handle(GetSaleQuery request, CancellationToken ct)
    {
        var sale = await db.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.Id == request.SaleId, ct)
            ?? throw new NotFoundException(nameof(Sale), request.SaleId);

        return Result<SaleDto>.Success(mapper.Map<SaleDto>(sale));
    }
}
