using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.UpdateTenantSettings;

public record UpdateTenantSettingsCommand(
    Guid CompanyId,
    int? MaxLocations = null,
    int? MaxEmployees = null,
    string? TimeZoneId = null,
    string? LogoUrl = null
) : IRequest<Result<bool>>;
