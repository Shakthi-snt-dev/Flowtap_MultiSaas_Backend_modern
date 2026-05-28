using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Commands.UpdatePaymentAccount;

public class UpdatePaymentAccountCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdatePaymentAccountCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdatePaymentAccountCommand request, CancellationToken ct)
    {
        var account = await db.PaymentAccounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.CompanyId == request.CompanyId, ct)
            ?? throw new NotFoundException("PaymentAccount", request.Id);

        if (request.Name is not null) account.Name = request.Name;
        if (request.IsActive.HasValue) account.IsActive = request.IsActive.Value;

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
