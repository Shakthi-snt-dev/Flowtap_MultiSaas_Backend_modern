namespace Flowtap_Application.Features.Sales.DTOs;

public record ClientDto(
    Guid Id, Guid CompanyId, Guid LocationId, string Type, string Name,
    string? Phone, string? Email, string? CompanyName, decimal DiscountPercent, bool IsActive,
    string? WhatsApp = null, string? GSTIN = null, string? Address = null,
    string? City = null, string? State = null, string? PostalCode = null,
    DateTime? DateOfBirth = null, string? ReferralSource = null, string? Notes = null);

public record SaleDto(
    Guid Id, Guid CompanyId, Guid LocationId, Guid ClientId, string? TransactionNumber,
    string Source, decimal SubTotal, decimal TaxAmount, decimal TotalAmount,
    string Status, DateTime CreatedAt, List<SaleItemDto> Items, List<PaymentDto> Payments);

public record SaleItemDto(
    Guid Id, Guid ProductId, string ProductName, string Type,
    decimal Quantity, decimal UnitPrice, decimal TaxPercent, decimal DiscountPercent, decimal Total);

public record PaymentDto(
    Guid Id, decimal Amount, string Method, string Purpose, DateTime PaidAt, string? ExternalReference);

public record CampaignDto(
    Guid Id, string Name, string Type, decimal DiscountValue, string DiscountType,
    DateTime StartDate, DateTime EndDate, string Status);

public record OfferDto(
    Guid Id, string PromoCode, decimal DiscountPercent, decimal MinOrderValue,
    int UsageLimit, int UsageCount, DateTime ValidFrom, DateTime ValidTo, bool IsActive);

public record PaymentAccountDto(Guid Id, string Name, string Type, Guid? LocationId, bool IsActive);

public record MethodMappingDto(
    Guid Id, string Method, Guid PaymentAccountId, string AccountName, string AccountType);

public record ClientListItemDto(
    Guid Id, string Name, string? Phone, string? Email, string Type, bool IsActive,
    Guid LocationId = default,
    decimal TotalSpent = 0, decimal TotalPaid = 0, int VisitCount = 0, DateTime? LastVisitAt = null,
    string? WhatsApp = null, string? CompanyName = null, string? GSTIN = null,
    string? Address = null, string? City = null, string? State = null, string? PostalCode = null,
    decimal DiscountPercent = 0, DateTime? DateOfBirth = null, string? ReferralSource = null,
    string? Notes = null);

public record ClientPurchaseDto(
    Guid Id, string? TransactionNumber, string Status, string Source,
    decimal SubTotal, decimal DiscountAmount, decimal TaxAmount, decimal TotalAmount,
    decimal TotalPaid, decimal BalanceDue,
    int ItemCount, DateTime CreatedAt,
    List<ClientPurchasePaymentDto> Payments);

public record ClientPurchasePaymentDto(
    Guid Id, string Method, decimal Amount, string Purpose, DateTime PaidAt);
