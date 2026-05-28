using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Flowtap_Presentation.Filters;

public class IndustryAccessFilter(IndustryType[] requiredIndustries, IApplicationDbContext context) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        var tenantIdClaim = ctx.HttpContext.User.FindFirstValue("tenantId");

        Guid tenantId;

        if (!Guid.TryParse(tenantIdClaim, out tenantId))
        {
            // tenantId not in JWT — token may have been issued before company setup.
            // Fall back: look up the user's CompanyId from AppUsers using the sub claim.
            var subClaim = ctx.HttpContext.User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
                        ?? ctx.HttpContext.User.FindFirstValue("sub");

            if (!Guid.TryParse(subClaim, out var userId))
            {
                ctx.Result = new ForbidResult();
                return;
            }

            var appUser = await context.AppUsers.AsNoTracking()
                .FirstOrDefaultAsync(a => a.UserAccountId == userId);

            if (appUser?.CompanyId == null)
            {
                ctx.Result = new ObjectResult(ApiResponse<object>.Fail("No company found for your account. Please complete company setup first."))
                { StatusCode = 403 };
                return;
            }

            tenantId = appUser.CompanyId.Value;
        }

        var tenant = await context.Tenants.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (tenant == null || !requiredIndustries.Contains(tenant.IndustryType))
        {
            ctx.Result = new ObjectResult(ApiResponse<object>.Fail("This feature is not available for your industry type."))
            { StatusCode = 403 };
            return;
        }

        await next();
    }
}
