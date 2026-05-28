using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.CreateMethodMapping;

public class CreateMethodMappingCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateMethodMappingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateMethodMappingCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<PaymentMethod>(request.Method, true, out var method))
            return Result<Guid>.Failure($"Invalid payment method: {request.Method}");

        // Verify account exists and belongs to this company
        var accountExists = await db.PaymentAccounts
            .AnyAsync(a => a.Id == request.PaymentAccountId && a.CompanyId == request.CompanyId && a.IsActive, ct);
        if (!accountExists)
            return Result<Guid>.Failure("Payment account not found or inactive.");

        // Upsert: remove existing mapping for same store+method
        var existing = await db.PaymentMethodMappings
            .FirstOrDefaultAsync(m =>
                m.CompanyId == request.CompanyId &&
                m.LocationId == request.LocationId &&
                m.Method == method, ct);

        if (existing is not null)
            db.PaymentMethodMappings.Remove(existing);

        var mapping = new PaymentMethodMapping
        {
            CompanyId        = request.CompanyId,
            LocationId       = request.LocationId,
            Method           = method,
            PaymentAccountId = request.PaymentAccountId,
        };
        db.PaymentMethodMappings.Add(mapping);
        await db.SaveChangesAsync(ct);

        return Result<Guid>.Success(mapping.Id);
    }
}
