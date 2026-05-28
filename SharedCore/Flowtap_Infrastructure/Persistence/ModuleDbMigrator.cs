using Microsoft.EntityFrameworkCore;

namespace Flowtap_Infrastructure.Persistence;

/// <summary>
/// Generic wrapper so each module can register its own DbContext as IModuleDbMigrator
/// without Flowtap_Infrastructure needing a compile-time reference to the module.
///
/// Usage in module extension (e.g. AddFoodModule):
///   services.AddScoped&lt;IModuleDbMigrator&gt;(sp =>
///       new ModuleDbMigrator&lt;FoodDbContext&gt;(sp.GetRequiredService&lt;FoodDbContext&gt;(), "Food"));
/// </summary>
public sealed class ModuleDbMigrator<TContext>(TContext context, string moduleName) : IModuleDbMigrator
    where TContext : Microsoft.EntityFrameworkCore.DbContext
{
    public string ModuleName { get; } = moduleName;

    public Task MigrateAsync(CancellationToken cancellationToken = default)
        => context.Database.MigrateAsync(cancellationToken);
}
