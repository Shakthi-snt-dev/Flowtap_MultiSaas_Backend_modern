using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Purchase.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Purchase.Queries.GetPurchaseOrder;

public class GetPurchaseOrderQueryHandler(IApplicationDbContext db, IMapper mapper)
    : IRequestHandler<GetPurchaseOrderQuery, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(GetPurchaseOrderQuery request, CancellationToken ct)
    {
        var order = await db.PurchaseOrders
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.CompanyId == request.CompanyId && p.Id == request.OrderId, ct)
            ?? throw new NotFoundException(nameof(Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities.PurchaseOrder), request.OrderId);

        return Result<PurchaseOrderDto>.Success(mapper.Map<PurchaseOrderDto>(order));
    }
}
