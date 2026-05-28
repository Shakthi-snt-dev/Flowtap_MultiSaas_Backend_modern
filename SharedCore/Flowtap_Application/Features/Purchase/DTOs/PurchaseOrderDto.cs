namespace Flowtap_Application.Features.Purchase.DTOs;

public record SupplierDto(
    Guid Id, Guid CompanyId, string Name, string? Category,
    string? ContactPerson, string Phone, string? Email, bool IsActive);

public record PurchaseOrderDto(
    Guid Id, Guid CompanyId, Guid SupplierId, string PONumber,
    string Status, decimal SubTotal, decimal TaxAmount, decimal TotalAmount,
    string Currency, string PaymentStatus, DateTime CreatedAt, List<PurchaseOrderItemDto> Items);

public record PurchaseOrderItemDto(
    Guid Id, Guid ProductId, int Quantity, int ReceivedQuantity,
    decimal UnitCost, decimal TaxPercent, string Status);

public record PurchaseReturnDto(
    Guid Id, Guid PurchaseOrderId, string ReturnNumber, string Status,
    decimal TotalAmount, DateTime CreatedAt);
