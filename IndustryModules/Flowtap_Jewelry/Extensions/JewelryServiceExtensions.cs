using Flowtap_Application.Common.Interfaces;
using Flowtap_Jewelry.DbContext;
using Flowtap_Jewelry.Industry;
using Flowtap_Infrastructure.Persistence;
using Flowtap_Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flowtap_Jewelry.Extensions;

public static class JewelryServiceExtensions
{
    public static IServiceCollection AddJewelryModule(this IServiceCollection services, IConfiguration configuration)
    {
        // JewelryDbContext connects to the jewelry-dedicated database.
        // Its migration creates ALL tables: core + jewelry. No conflict — separate DB.
        services.AddDbContext<JewelryDbContext>((sp, options) =>
            options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsql => npgsql.MigrationsAssembly("Flowtap_Jewelry"))
                .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<IJewelryDbContext>(sp => sp.GetRequiredService<JewelryDbContext>());

        services.AddScoped<IModuleDbMigrator>(sp =>
            new ModuleDbMigrator<JewelryDbContext>(sp.GetRequiredService<JewelryDbContext>(), "Jewelry"));

        services.AddControllers()
                .AddApplicationPart(typeof(JewelryServiceExtensions).Assembly);

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(JewelryServiceExtensions).Assembly));

        services.AddScoped<IIndustryDataSeeder, JewelrySeedService>();

        return services;
    }
}
