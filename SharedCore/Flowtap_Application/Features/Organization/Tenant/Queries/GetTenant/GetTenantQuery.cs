using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Organization.Tenant.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Tenant.Queries.GetTenant;

public record GetTenantQuery(Guid TenantId) : IRequest<Result<TenantDto>>;
