using Flowtap_Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;

namespace Flowtap_Configuration.DependencyInjection;

public static class PresentationServiceExtensions
{
    private static readonly string[] Modules =
    [
        "POS", "Inventory", "ServiceTickets", "Purchasing",
        "Clients", "Employees", "Reports", "Settings"
    ];

    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
                .AddApplicationPart(System.Reflection.Assembly.Load("Flowtap_Presentation.Core"));
        services.AddSignalR();

        // ── Permission-based authorization ────────────────────────────────────
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddAuthorization(options =>
        {
            foreach (var module in Modules)
            {
                options.AddPolicy(
                    $"{RequirePermissionAttribute.PolicyPrefix}{module}",
                    policy => policy
                        .RequireAuthenticatedUser()
                        .AddRequirements(new PermissionRequirement(module)));
            }
        });
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Flowtap API",
                Version = "v1",
                Description = "Multi-tenant POS + ERP SaaS API for Repair, Jewellery, Supermarket, Restaurant and Hotel industries.",
                Contact = new OpenApiContact { Name = "Flowtap Team" }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste your JWT token only — do NOT add 'Bearer' prefix. Swagger adds it automatically."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        var corsOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                if (corsOrigins != null && corsOrigins.Length > 0)
                {
                    policy.WithOrigins("http://localhost:5173",
                        "http://localhost:3000","http://localhost:3005",
                        "https://app.flowtap.io")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
                else
                {
                    policy.SetIsOriginAllowed(_ => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
            });
        });

        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", limiter =>
            {
                limiter.PermitLimit = 100;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit = 10;
            });
            options.RejectionStatusCode = 429;
        });

        return services;
    }
}
