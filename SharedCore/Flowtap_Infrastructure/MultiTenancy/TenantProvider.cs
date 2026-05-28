using Microsoft.AspNetCore.Http;

namespace Flowtap_Infrastructure.MultiTenancy;

public class TenantProvider(IHttpContextAccessor httpContextAccessor)
{
    public Guid? GetTenantId()
    {
        var claim = httpContextAccessor.HttpContext?.User?.FindFirst("tenantId");
        return claim is not null && Guid.TryParse(claim.Value, out var id) ? id : null;
    }
}
