using Flowtap_Application.Common.Interfaces;
using Flowtap_Hotel.DbContext;
using Flowtap_Hotel.Industry;
using Flowtap_Infrastructure.Persistence;
using Flowtap_Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flowtap_Hotel.Extensions;

public static class HotelServiceExtensions
{
    public static IServiceCollection AddHotelModule(this IServiceCollection services, IConfiguration configuration)
    {
        // HotelDbContext connects to the hotel-dedicated database.
        // Its migration creates ALL tables: core + hotel. No conflict — separate DB.
        services.AddDbContext<HotelDbContext>((sp, options) =>
            options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsql => npgsql.MigrationsAssembly("Flowtap_Hotel"))
                .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<IHotelDbContext>(sp => sp.GetRequiredService<HotelDbContext>());

        services.AddScoped<IModuleDbMigrator>(sp =>
            new ModuleDbMigrator<HotelDbContext>(sp.GetRequiredService<HotelDbContext>(), "Hotel"));

        services.AddControllers()
                .AddApplicationPart(typeof(HotelServiceExtensions).Assembly);

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(HotelServiceExtensions).Assembly));

        services.AddScoped<IIndustryDataSeeder, HotelSeedService>();

        return services;
    }
}
