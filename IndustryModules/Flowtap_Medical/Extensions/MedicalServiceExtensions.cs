using Flowtap_Application.Common.Interfaces;
using Flowtap_Medical.DbContext;
using Flowtap_Medical.Industry;
using Flowtap_Infrastructure.Persistence;
using Flowtap_Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flowtap_Medical.Extensions;

public static class MedicalServiceExtensions
{
    public static IServiceCollection AddMedicalModule(this IServiceCollection services, IConfiguration configuration)
    {
        // MedicalDbContext connects to the medical-dedicated database.
        // Its migration creates ALL tables: core + medical. No conflict — separate DB.
        services.AddDbContext<MedicalDbContext>((sp, options) =>
            options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsql => npgsql.MigrationsAssembly("Flowtap_Medical"))
                .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<IMedicalDbContext>(sp => sp.GetRequiredService<MedicalDbContext>());

        services.AddScoped<IModuleDbMigrator>(sp =>
            new ModuleDbMigrator<MedicalDbContext>(sp.GetRequiredService<MedicalDbContext>(), "Medical"));

        services.AddControllers()
                .AddApplicationPart(typeof(MedicalServiceExtensions).Assembly);

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(MedicalServiceExtensions).Assembly));

        services.AddScoped<IIndustryDataSeeder, MedicalSeedService>();

        return services;
    }
}
