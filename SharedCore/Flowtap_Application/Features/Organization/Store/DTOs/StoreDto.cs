namespace Flowtap_Application.Features.Organization.Store.DTOs;
public record StoreDto(Guid Id, Guid CompanyId, string Title, string Phone, string Address, string CountryCode, string CurrencyCode, string TimeZoneId, string? LocationCode, bool IsActive);
public record StoreListItemDto(Guid Id, string Title, string Phone, string Address, string CountryCode, string CurrencyCode, string TimeZoneId, string? LocationCode, bool IsActive);
