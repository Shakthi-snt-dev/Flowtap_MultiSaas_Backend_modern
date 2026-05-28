using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Queries.GetPaymentAccounts;

public class GetPaymentAccountsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetPaymentAccountsQuery, Result<List<PaymentAccountDto>>>
{
    public async Task<Result<List<PaymentAccountDto>>> Handle(GetPaymentAccountsQuery request, CancellationToken ct)
    {
        var query = db.PaymentAccounts
            .Where(a => a.CompanyId == request.CompanyId && a.IsActive);

        var locationId = request.LocationId ?? currentUser.StoreId;
        if (locationId.HasValue && locationId.Value != Guid.Empty)
            query = query.Where(a => a.LocationId == null || a.LocationId == locationId.Value);

        var items = await query.OrderBy(a => a.Name).ToListAsync(ct);

        var dtos = items.Select(a => new PaymentAccountDto(
            a.Id, a.Name, a.Type.ToString(), a.LocationId, a.IsActive)).ToList();

        return Result<List<PaymentAccountDto>>.Success(dtos);
    }
}
