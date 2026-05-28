using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Organization.Tenant.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Tenant.Queries.GetTenant;

public class GetTenantQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetTenantQuery, Result<TenantDto>>
{
    public async Task<Result<TenantDto>> Handle(GetTenantQuery request, CancellationToken ct)
    {
        Flowtap_Domain.BoundedContexts.Core.Organization.Entities.Tenant? tenant = null;

        if (request.TenantId != Guid.Empty)
        {
            tenant = await db.Tenants
                .Include(t => t.Settings)
                .FirstOrDefaultAsync(t => t.Id == request.TenantId && t.IsActive, ct);
        }

        // Fallback: fresh-signup JWT has no companyId claim yet
        if (tenant == null && currentUser.UserId.HasValue)
        {
            var appUser = await db.AppUsers
                .FirstOrDefaultAsync(a => a.UserAccountId == currentUser.UserId.Value, ct);
            if (appUser?.CompanyId != null)
            {
                tenant = await db.Tenants
                    .Include(t => t.Settings)
                    .FirstOrDefaultAsync(t => t.Id == appUser.CompanyId && t.IsActive, ct);
            }
        }

        if (tenant == null)
            return Result<TenantDto>.Failure("Tenant not found.");

        return Result<TenantDto>.Success(new TenantDto(
            tenant.Id, tenant.Title, tenant.Phone, tenant.Email,
            tenant.Country, tenant.Currency, tenant.SubDomain, tenant.BusinessType,
            tenant.IndustryType.ToString(), tenant.IsActive,
            tenant.Settings?.IsOnboardingComplete ?? false,
            tenant.ActiveModules,
            tenant.Settings?.MaxLocations ?? 1,
            tenant.Settings?.MaxEmployees ?? 5,
            tenant.Settings?.TimeZoneId,
            tenant.Settings?.LogoUrl));
    }
}
