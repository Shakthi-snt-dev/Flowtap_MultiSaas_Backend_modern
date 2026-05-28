using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.VoidSale;

public class VoidSaleCommandHandler(IApplicationDbContext db)
    : IRequestHandler<VoidSaleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(VoidSaleCommand request, CancellationToken ct)
    {
        var sale = await db.Sales
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.Id == request.SaleId, ct)
            ?? throw new NotFoundException(nameof(Sale), request.SaleId);

        sale.Status = SaleStatus.Cancelled;
        sale.CancellationReason = request.Reason;

        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(sale.Id);
    }
}
