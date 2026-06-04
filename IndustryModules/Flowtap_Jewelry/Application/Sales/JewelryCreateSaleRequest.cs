using Flowtap_Application.Features.Sales.Commands.CreateSale;

namespace Flowtap_Jewelry.Application.Sales;

/// <summary>
/// Jewelry-industry sale request — exposes common POS fields + hallmark/exchange context.
/// No food tables, no repair tickets.
/// </summary>
public record JewelryCreateSaleRequest(
    Guid LocationId,
    Guid? ClientId,
    string Source,
    string? Notes,
    string? IdempotencyKey,
    List<CreateSaleItemDto> Items,
    List<CreateSalePaymentDto>? Payments = null,
    Guid? EmployeeId = null,
    // Jewelry-specific context
    string? HallmarkDetails = null,
    string? ExchangeReference = null
);
