using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.AddPayment;

public record AddPaymentCommand(
    Guid CompanyId,
    Guid SaleId,
    decimal Amount,
    string Method,
    string Purpose,
    Guid AccountId,             // Guid.Empty = auto-resolve via PaymentMethodMapping
    string? ExternalReference,
    string? Comment,
    Guid? EmployeeId,
    string? IdempotencyKey) : IRequest<Result<Guid>>;
