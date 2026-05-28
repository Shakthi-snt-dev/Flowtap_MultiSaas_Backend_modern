using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Queries.GetPaymentAccounts;

public record GetPaymentAccountsQuery(Guid CompanyId, Guid? LocationId = null)
    : IRequest<Result<List<PaymentAccountDto>>>;
