using Flowtap_Application.Common.Interfaces;
using Flowtap_Infrastructure.Persistence;
using Flowtap_Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Industry;

namespace Flowtap_Repair.Extensions;

public static class RepairServiceExtensions
{
    public static IServiceCollection AddRepairModule(this IServiceCollection services, IConfiguration configuration)
    {
        // RepairDbContext connects to the repair-dedicated database.
        // Its migration creates ALL tables: core + repair. No conflict — separate DB.
        services.AddDbContext<RepairDbContext>((sp, options) =>
            options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsql => npgsql.MigrationsAssembly("Flowtap_Repair"))
                .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<IRepairDbContext>(sp => sp.GetRequiredService<RepairDbContext>());

        // Register IModuleDbMigrator so DatabaseSeeder migrates repair tables at startup
        services.AddScoped<IModuleDbMigrator>(sp =>
            new ModuleDbMigrator<RepairDbContext>(sp.GetRequiredService<RepairDbContext>(), "Repair"));

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(RepairServiceExtensions).Assembly));

        services.AddControllers()
                .AddApplicationPart(typeof(RepairServiceExtensions).Assembly);

        services.AddScoped<IIndustryDataSeeder, RepairSeedService>();

        return services;
    }
}
