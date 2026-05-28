using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.DeletePaymentAccount;

public record DeletePaymentAccountCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;
