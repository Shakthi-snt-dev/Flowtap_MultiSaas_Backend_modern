using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.Commands.CreateSale;
using Flowtap_Food.Application.Behaviors;
using Flowtap_Food.DbContext;
using Flowtap_Food.Industry;
using Flowtap_Infrastructure.Persistence;
using Flowtap_Infrastructure.Persistence.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flowtap_Food.Extensions;

public static class FoodServiceExtensions
{
    public static IServiceCollection AddFoodModule(this IServiceCollection services, IConfiguration configuration)
    {
        // FoodDbContext connects to the same DB as ApplicationDbContext (same connection string).
        // Its migration creates ALL tables: core (120) + food (6) = 126 total.
        // Since Food_API has its own dedicated database, there is no conflict with other modules.
        services.AddDbContext<FoodDbContext>((sp, options) =>
            options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsql => npgsql.MigrationsAssembly("Flowtap_Food"))
                .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

        // Register IFoodDbContext → FoodDbContext
        services.AddScoped<IFoodDbContext>(sp => sp.GetRequiredService<FoodDbContext>());

        // Register IModuleDbMigrator so DatabaseSeeder migrates food tables at startup
        services.AddScoped<IModuleDbMigrator>(sp =>
            new ModuleDbMigrator<FoodDbContext>(sp.GetRequiredService<FoodDbContext>(), "Food"));

        // Register food controllers (FoodController, StockAlertController)
        services.AddControllers()
                .AddApplicationPart(typeof(FoodServiceExtensions).Assembly);

        // Register all MediatR handlers in this assembly
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(FoodServiceExtensions).Assembly));

        // FoodSaleBehavior — wraps CreateSaleCommand to auto-create KOTs
        services.AddTransient(
            typeof(IPipelineBehavior<CreateSaleCommand, Result<Guid>>),
            typeof(FoodSaleBehavior));

        // Industry seed service
        services.AddScoped<IIndustryDataSeeder, FoodSeedService>();

        return services;
    }
}
