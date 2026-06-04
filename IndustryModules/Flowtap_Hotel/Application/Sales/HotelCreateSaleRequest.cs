using Flowtap_Application.Features.Sales.Commands.CreateSale;

namespace Flowtap_Hotel.Application.Sales;

/// <summary>
/// Hotel-industry sale request — exposes common POS fields + hotel context (room, booking).
/// No food tables, no repair tickets.
/// </summary>
public record HotelCreateSaleRequest(
    Guid LocationId,
    Guid? ClientId,
    string Source,
    string? Notes,
    string? IdempotencyKey,
    List<CreateSaleItemDto> Items,
    List<CreateSalePaymentDto>? Payments = null,
    Guid? EmployeeId = null,
    // Hotel-specific context
    Guid? RoomId = null,
    string? BookingReference = null
);
