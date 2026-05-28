using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.UpdateTenant;

public record UpdateTenantCommand(
    Guid CompanyId, string? Title, string? Phone, string? Email,
    string? Website, string? Address) : IRequest<Result<bool>>;
