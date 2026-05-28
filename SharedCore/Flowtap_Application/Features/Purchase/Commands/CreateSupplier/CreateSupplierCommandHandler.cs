using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Purchase.Entities;
using MediatR;

namespace Flowtap_Application.Features.Purchase.Commands.CreateSupplier;

public class CreateSupplierCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateSupplierCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSupplierCommand request, CancellationToken ct)
    {
        var supplier = new Supplier
        {
            CompanyId = request.CompanyId,
            LocationId = request.LocationId,
            Name = request.Name,
            Category = request.Category,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
            Email = request.Email,
            GSTIN = request.GSTIN
        };
        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(supplier.Id);
    }
}
