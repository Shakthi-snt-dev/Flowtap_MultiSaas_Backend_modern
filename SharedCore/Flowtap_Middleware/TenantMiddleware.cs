using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Subscription.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flowtap_Middleware;

/// <summary>
/// Validates that the authenticated user has a valid tenantId claim.
/// Skips paths that don't require tenant context (auth routes).
/// Also checks active subscription or trial — returns 402 if both are expired.
/// </summary>
public class TenantMiddleware(
    RequestDelegate next,
    ILogger<TenantMiddleware> logger,
    IServiceScopeFactory scopeFactory)
{
    private static readonly HashSet<string> _skipPaths =
    [
        "/api/v1/auth/register",
        "/api/v1/auth/login",
        "/api/v1/auth/refresh",
        "/api/v1/auth/verify-email",
        "/api/v1/auth/forgot-password",
        "/api/v1/auth/reset-password",
        "/api/v1/auth/resend-verification",
        "/swagger",
        "/health"
    ];

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // Skip tenant validation for public endpoints
        if (_skipPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await next(context);
            return;
        }

        // For authenticated requests that target tenant resources, validate tenantId claim
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenantId");
            if (tenantClaim is not null && Guid.TryParse(tenantClaim.Value, out var tenantId))
            {
                logger.LogDebug("Request for tenant {TenantId} at {Path}", tenantClaim.Value, path);

                // Check for active subscription or active trial
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var now = DateTime.UtcNow;

                var hasActiveSubscription = await db.CompanySubscriptions
                    .AnyAsync(s => s.CompanyId == tenantId &&
                                   s.Status == SubscriptionStatus.Active &&
                                   s.EndDate > now, context.RequestAborted);

                if (!hasActiveSubscription)
                {
                    var hasActiveTrial = await db.TrialPlans
                        .AnyAsync(t => t.CompanyId == tenantId &&
                                       !t.IsExpired &&
                                       t.TrialEndDate > now, context.RequestAborted);

                    if (!hasActiveTrial)
                    {
                        context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            "{\"error\":\"Your subscription or trial has expired. Please renew to continue.\"}",
                            context.RequestAborted);
                        return;
                    }
                }
            }
            // If no tenantId on token, allow through — tenant-bound endpoints
            // will check via ICurrentUserService and return 403 themselves
        }

        await next(context);
    }
}
