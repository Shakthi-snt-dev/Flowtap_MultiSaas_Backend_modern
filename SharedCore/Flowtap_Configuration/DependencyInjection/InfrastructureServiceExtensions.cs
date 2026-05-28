using Flowtap_Application.Common.Interfaces;
using Flowtap_Infrastructure.Settings;
using Flowtap_Infrastructure.BackgroundJobs;
using Flowtap_Infrastructure.Identity;
using Flowtap_Infrastructure.MultiTenancy;
using Flowtap_Infrastructure.Persistence.DbContext;
using Flowtap_Infrastructure.Persistence.Interceptors;
using Flowtap_Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Hangfire;
using Hangfire.PostgreSql;

namespace Flowtap_Configuration.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<SmsSettings>(configuration.GetSection("SmsSettings"));
        services.Configure<RedisSettings>(configuration.GetSection("RedisSettings"));
        services.Configure<StorageSettings>(configuration.GetSection("StorageSettings"));
        services.Configure<SubscriptionSettings>(configuration.GetSection("SubscriptionSettings"));

        // Audit interceptor (registered before DbContext so it can be resolved)
        services.AddScoped<AuditSaveChangesInterceptor>();

        // Database
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
            options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsql => npgsql.MigrationsAssembly("Flowtap_Infrastructure"))
                .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        // Redis Cache
        var redisConn = configuration.GetSection("RedisSettings:ConnectionString").Value
                        ?? "localhost:6379";
        var redisInstance = configuration.GetSection("RedisSettings:InstanceName").Value
                            ?? "flowtap:";

        // abortConnect=false: don't throw at startup if Redis is unreachable
        var redisConnWithOptions = $"{redisConn},abortConnect=false";

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnWithOptions;
            options.InstanceName = redisInstance;
        });

        // Lazy singleton: connection is created on first use, not at DI build time
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnWithOptions));

        // JWT Authentication
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                          ?? new JwtSettings();
        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(5)
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Serilog.Log.Warning(context.Exception, "JWT Authentication failed.");
                    return Task.CompletedTask;
                }
            };
        });

        // HttpContext
        services.AddHttpContextAccessor();

        // Application services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IWhatsAppService, WhatsAppService>();

        // Named HttpClients for external messaging APIs
        services.AddHttpClient("Twilio");    // SMS via Twilio REST API
        services.AddHttpClient("WhatsApp"); // WhatsApp via Meta Cloud API
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<TenantProvider>();
        services.AddScoped<TaxCalculationService>();
        services.AddScoped<ITaxTemplateService, TaxTemplateService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();

        // Background jobs
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => 
                options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"))));

        services.AddHangfireServer();

        services.AddHostedService<EmailDispatchService>();
        services.AddHostedService<SmsDispatchService>();
        services.AddHostedService<WhatsAppDispatchService>();
        services.AddHostedService<ReorderCheckService>();
        services.AddHostedService<SubscriptionExpiryService>();

        return services;
    }
}
