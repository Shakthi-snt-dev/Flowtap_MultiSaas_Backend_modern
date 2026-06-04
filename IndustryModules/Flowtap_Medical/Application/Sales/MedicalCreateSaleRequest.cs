using Flowtap_Application.Features.Sales.Commands.CreateSale;

namespace Flowtap_Medical.Application.Sales;

/// <summary>
/// Medical-industry sale request — exposes common POS fields + patient/prescription context.
/// No food tables, no repair tickets.
/// </summary>
public record MedicalCreateSaleRequest(
    Guid LocationId,
    Guid? ClientId,
    string Source,
    string? Notes,               // Use for prescription reference
    string? IdempotencyKey,
    List<CreateSaleItemDto> Items,
    List<CreateSalePaymentDto>? Payments = null,
    Guid? EmployeeId = null,
    // Medical-specific context
    Guid? PatientId = null,
    string? AppointmentNumber = null
);
