using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.UpdatePaymentAccount;

public record UpdatePaymentAccountCommand(
    Guid Id, Guid CompanyId, string? Name, bool? IsActive) : IRequest<Result<bool>>;
