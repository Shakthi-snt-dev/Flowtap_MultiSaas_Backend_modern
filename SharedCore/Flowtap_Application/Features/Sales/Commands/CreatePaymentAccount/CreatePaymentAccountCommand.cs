using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.CreatePaymentAccount;

public record CreatePaymentAccountCommand(
    Guid CompanyId, string Name, string Type, Guid? LocationId = null) : IRequest<Result<Guid>>;
