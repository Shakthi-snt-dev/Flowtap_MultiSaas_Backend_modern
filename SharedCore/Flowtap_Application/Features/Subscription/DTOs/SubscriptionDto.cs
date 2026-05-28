namespace Flowtap_Application.Features.Subscription.DTOs;

public record PlanDto(
    Guid Id, string Name, string BillingCycle, decimal PricePerLocation,
    int BillingCycleInDays, int MaxLocations, int MaxEmployees, bool IsActive);

public record SubscriptionDto(
    Guid Id, Guid CompanyId, Guid SubscriptionPlanId, string PlanName,
    DateTime StartDate, DateTime EndDate, DateTime NextBillingDate,
    string Status, bool IsActive, int TotalLocations, decimal TotalAmount);

public record TrialDto(
    Guid Id, Guid CompanyId, int TrialDays, DateTime TrialStartDate,
    DateTime TrialEndDate, bool IsExpired, int LocationCount, bool IsConverted);

public record InvoiceDto(
    Guid Id, string InvoiceNumber, DateTime InvoiceDate, decimal TotalAmount,
    string Currency, bool IsPaid);
