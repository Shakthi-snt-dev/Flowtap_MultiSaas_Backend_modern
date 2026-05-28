using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Flowtap_Middleware;

public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        var method = context.Request.Method;
        var path = context.Request.Path;

        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();
            var statusCode = context.Response.StatusCode;
            // Read tenantId AFTER next() so UseAuthentication has already run
            // and context.User is populated with JWT claims.
            var tenantId = context.User.FindFirst("tenantId")?.Value ?? "-";

            logger.LogInformation(
                "{Method} {Path} → {StatusCode} | {Elapsed}ms | Tenant: {TenantId}",
                method, path, statusCode, sw.ElapsedMilliseconds, tenantId);
        }
    }
}
