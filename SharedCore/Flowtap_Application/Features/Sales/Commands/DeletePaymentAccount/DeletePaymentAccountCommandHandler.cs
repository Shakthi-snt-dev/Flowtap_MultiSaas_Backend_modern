using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.DeletePaymentAccount;

public class DeletePaymentAccountCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeletePaymentAccountCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeletePaymentAccountCommand request, CancellationToken ct)
    {
        var account = await db.PaymentAccounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException("PaymentAccount", request.Id);

        // Guard: cannot delete if active method mappings reference this account
        var hasMappings = await db.PaymentMethodMappings
            .AnyAsync(m => m.CompanyId == request.CompanyId && m.PaymentAccountId == request.Id, ct);

        if (hasMappings)
            return Result<bool>.Failure("Cannot delete account while it is mapped to a payment method. Remove all method mappings first.");

        account.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
