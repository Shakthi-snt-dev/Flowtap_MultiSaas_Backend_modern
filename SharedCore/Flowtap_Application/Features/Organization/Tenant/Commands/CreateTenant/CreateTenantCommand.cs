using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Tenant.Commands.CreateTenant;

public record CreateTenantCommand(
    string Title, string Phone, string? Email, string Country,
    string Currency, string SubDomain, string BusinessType, string IndustryType) : IRequest<Result<Guid>>;
