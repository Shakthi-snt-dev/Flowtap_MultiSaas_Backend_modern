using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.CreatePaymentAccount;

public class CreatePaymentAccountCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreatePaymentAccountCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePaymentAccountCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<PaymentAccountType>(request.Type, true, out var accountType))
            return Result<Guid>.Failure($"Invalid account type: {request.Type}");

        var account = new PaymentAccount
        {
            CompanyId = request.CompanyId,
            Name = request.Name,
            Type = accountType,
            LocationId = request.LocationId,
            IsActive = true
        };

        db.PaymentAccounts.Add(account);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(account.Id);
    }
}
