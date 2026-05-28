using Flowtap_Application.Common.DTOs;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.CreateSale;

public record CreateSaleCommand(
    Guid CompanyId,
    Guid LocationId,
    Guid? ClientId,             // nullable — null = walk-in customer
    string Source,
    Guid? TicketId,
    string? Notes,
    string? IdempotencyKey,
    List<CreateSaleItemDto> Items,
    List<CreateSalePaymentDto>? Payments = null,  // inline payments at checkout
    Guid? EmployeeId = null,                      // active cashier who processed the sale
    // When creating a sale from a ticket, the Repair module passes the advance already
    // collected on the ticket so the Sales handler can compute the correct status
    // without querying ServiceTickets (keeping this project free of Repair dependency).
    decimal TicketPrepayment = 0,
    string? TicketNumber = null,
    // Food industry extensions — null for all other industries
    Guid? TableId = null,
    FoodOrderType? FoodOrderType = null
) : IRequest<Result<Guid>>;

public record CreateSaleItemDto(
    Guid ProductId,
    string ProductName,
    string Type,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxPercent,
    decimal DiscountPercent,
    decimal DiscountAmount,
    string? SerialNumber = null);   // populated when item was added via serial scan

public record CreateSalePaymentDto(
    string Method,              // Cash | Card | UPI | NetBanking | Wallet
    decimal Amount,
    string Purpose = "Final",   // Advance | Partial | Final
    string? Reference = null);  // external ref / transaction ID
