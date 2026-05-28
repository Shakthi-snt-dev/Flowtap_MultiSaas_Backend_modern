using System.Net;
using System.Text.Json;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Flowtap_Middleware;

public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException vex =>
                (HttpStatusCode.BadRequest, "One or more validation errors occurred.",
                 vex.Errors.SelectMany(e => e.Value).ToList()),

            NotFoundException nex =>
                (HttpStatusCode.NotFound, nex.Message, (List<string>)[]),

            UnauthorizedException uex =>
                (HttpStatusCode.Unauthorized, uex.Message, (List<string>)[]),

            ForbiddenException fex =>
                (HttpStatusCode.Forbidden, fex.Message, (List<string>)[]),

            ConflictException cex =>
                (HttpStatusCode.Conflict, cex.Message, (List<string>)[]),

            _ => (HttpStatusCode.InternalServerError,
                  "An unexpected error occurred. Please try again later.",
                  (List<string>)[])
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);

        var response = errors.Count > 0
            ? ApiResponse<object>.Fail(errors, context.TraceIdentifier)
            : ApiResponse<object>.Fail(message, context.TraceIdentifier);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions));
    }
}
