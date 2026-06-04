using Flowtap_Application.Features.Sales.Commands.CreateSale;

namespace Flowtap_Repair.Application.Sales;

/// <summary>
/// Repair-industry sale request — only exposes fields relevant to repair shop POS.
/// TicketId / TicketPrepayment / TicketNumber are repair-specific; food/hotel/medical never see these.
/// Mapped to CreateSaleCommand in RepairSaleController.
/// </summary>
public record RepairCreateSaleRequest(
    Guid LocationId,
    Guid? ClientId,
    string Source,
    string? Notes,
    string? IdempotencyKey,
    List<CreateSaleItemDto> Items,
    List<CreateSalePaymentDto>? Payments = null,
    Guid? EmployeeId = null,
    // Repair-specific fields
    Guid? TicketId = null,
    decimal TicketPrepayment = 0,
    string? TicketNumber = null
);
