using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;

namespace Flowtap_Application.Common.Interfaces;

public interface ITenantService
{
    Task<Tenant?> GetTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<bool> ValidateTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<bool> IsTrialAsync(Guid tenantId, CancellationToken ct = default);
    Task<bool> IsSubscriptionActiveAsync(Guid tenantId, CancellationToken ct = default);
}
