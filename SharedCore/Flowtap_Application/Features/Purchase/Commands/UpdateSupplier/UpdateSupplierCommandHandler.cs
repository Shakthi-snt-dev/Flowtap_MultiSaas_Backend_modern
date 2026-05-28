using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Purchase.Commands.UpdateSupplier;

public class UpdateSupplierCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateSupplierCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateSupplierCommand request, CancellationToken ct)
    {
        var supplier = await db.Suppliers
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.Id == request.SupplierId, ct)
            ?? throw new NotFoundException(nameof(Supplier), request.SupplierId);

        supplier.Name = request.Name;
        supplier.Category = request.Category;
        supplier.ContactPerson = request.ContactPerson;
        supplier.Phone = request.Phone;
        supplier.Email = request.Email;
        supplier.GSTIN = request.GSTIN;

        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(supplier.Id);
    }
}
