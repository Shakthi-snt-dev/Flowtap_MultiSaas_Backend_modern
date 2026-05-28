namespace Flowtap_Application.Features.Organization.Tenant.DTOs;
public record TenantDto(
    Guid Id, string Title, string Phone, string? Email,
    string Country, string Currency, string SubDomain,
    string BusinessType, string IndustryType, bool IsActive,
    bool IsOnboardingComplete, string? ActiveModules,
    // TenantSettings fields
    int MaxLocations, int MaxEmployees, string? TimeZoneId, string? LogoUrl);
public record BusinessProfileDto(string IndustryType, string BusinessType, string Currency, string Country, string? TimeZoneId);
public record OnboardingDto(bool IsComplete, string? IndustryType, bool HasStore, bool HasEmployee);
