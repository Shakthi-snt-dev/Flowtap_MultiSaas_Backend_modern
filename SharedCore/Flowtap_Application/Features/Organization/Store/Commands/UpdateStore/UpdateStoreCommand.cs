using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Store.Commands.UpdateStore;

public record UpdateStoreCommand(
    Guid Id, Guid CompanyId, string? Title, string? Phone,
    string? Address, string? TimeZoneId, string? CountryCode, string? CurrencyCode,
    bool? IsActive, string? LocationCode = null,
    Guid? ManagerEmployeeId = null) : IRequest<Result<bool>>;
