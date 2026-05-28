using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Purchase.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Purchase.Queries.GetSupplier;

public class GetSupplierQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetSupplierQuery, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(GetSupplierQuery request, CancellationToken ct)
    {
        var supplier = await db.Suppliers
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.Id == request.SupplierId, ct)
            ?? throw new NotFoundException(nameof(Supplier), request.SupplierId);

        return Result<SupplierDto>.Success(mapper.Map<SupplierDto>(supplier));
    }
}
