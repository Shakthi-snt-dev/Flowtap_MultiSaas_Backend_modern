using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Queries.GetPayments;

public record GetPaymentsQuery(
    Guid CompanyId,
    Guid? LocationId = null,
    Guid? TicketId   = null,
    Guid? SaleId     = null,
    string? Method   = null,
    string? Purpose  = null,
    DateTime? DateFrom = null,
    DateTime? DateTo   = null,
    int Page     = 1,
    int PageSize = 30
) : IRequest<Result<PaginatedList<PaymentListItemDto>>>;

public record PaymentListItemDto(
    Guid    Id,
    decimal Amount,
    string  Method,
    string  Purpose,
    string  AccountName,
    string  AccountType,
    Guid?   TicketId,
    string? TicketNumber,
    Guid?   SaleId,
    string? SaleTransactionNumber,
    string? ExternalReference,
    string? Comment,
    DateTime PaidAt);
