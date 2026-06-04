using Flowtap_Application.Common.Interfaces;
using Flowtap_Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace Flowtap_Configuration.DependencyInjection;

public static class PresentationServiceExtensions
{
    // Core shared modules — always present regardless of industry.
    // Industry modules register their own permission names via IIndustryPermissionModule
    // in their ServiceExtensions (e.g. FoodServiceExtensions.AddFoodModule).
    private static readonly string[] CoreModules =
    [
        "POS", "Inventory", "ServiceTickets", "Purchasing",
        "Clients", "Employees", "Reports", "Settings"
    ];

    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
                .AddApplicationPart(System.Reflection.Assembly.Load("Flowtap_Presentation.Core"))
                .AddJsonOptions(o =>
                {
                    // Accept enum values as strings (e.g. "DineIn", "Final") — integers still work
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        services.AddSignalR();

        // ── Permission-based authorization ────────────────────────────────────
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // Core module policies — always present
        services.AddAuthorization(options =>
        {
            foreach (var module in CoreModules)
            {
                options.AddPolicy(
                    $"{RequirePermissionAttribute.PolicyPrefix}{module}",
                    policy => policy
                        .RequireAuthenticatedUser()
                        .AddRequirements(new PermissionRequirement(module)));
            }
        });

        // Industry module policies — registered lazily via PostConfigure so that
        // modules added AFTER AddPresentationServices() are still discovered.
        services.AddSingleton<IPostConfigureOptions<AuthorizationOptions>,
            IndustryPermissionPostConfigure>();
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
                    // Production: restrict to explicit origins — required for AllowCredentials()
                    // AllowAnyOrigin() cannot be combined with AllowCredentials() — ASP.NET throws
                    policy.WithOrigins(corsOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
                else
                {
                    // Development / no config: allow all origins
                    // Must use SetIsOriginAllowed (not AllowAnyOrigin) to keep AllowCredentials()
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
