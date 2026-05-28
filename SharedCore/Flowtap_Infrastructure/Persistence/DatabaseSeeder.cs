using Flowtap_Infrastructure.Persistence.DbContext;
using Flowtap_Infrastructure.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flowtap_Infrastructure.Persistence;

/// <summary>
/// Runs once at application startup to migrate and seed the database.
/// Handles two deployment scenarios automatically:
///
/// ── Standalone industry API (e.g. Flowtap_Food_API) ──────────────────────────
///   Each industry API has its OWN dedicated database.
///   The module DbContext (FoodDbContext, RepairDbContext, etc.) creates ALL tables:
///   core tables (120) + module-specific tables.
///   ApplicationDbContext.MigrateAsync() is skipped — the module context handles everything.
///
/// ── Flowtap_Presentation (full SaaS, all industries) ─────────────────────────
///   Has its own main database that must contain ALL tables from ALL modules.
///   ApplicationDbContext.MigrateAsync() runs first (core tables).
///   Each module migrator then runs its own migration against the same DB.
///   Flowtap_Presentation uses separate migration files per context to avoid
///   re-creating core tables (see plan for details on Presentation migrations).
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        await using var scope = services.CreateAsyncScope();
        var sp = scope.ServiceProvider;

        var moduleMigrators = sp.GetServices<IModuleDbMigrator>().ToList();

        if (moduleMigrators.Count == 0)
        {
            // ── Flowtap_Presentation or core-only deployment ──────────────────
            // No module migrators registered → migrate core tables via ApplicationDbContext.
            var db = sp.GetRequiredService<ApplicationDbContext>();
            try
            {
                logger.LogInformation("Applying core migrations (ApplicationDbContext)...");
                await db.Database.MigrateAsync();
                logger.LogInformation("Core migrations applied.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error applying core migrations.");
                throw;
            }

            await SeedPermissionsAsync(db, logger);
        }
        else
        {
            // ── Standalone industry API ───────────────────────────────────────
            // Module DbContext migration creates ALL tables (core + module).
            // ApplicationDbContext.MigrateAsync() is skipped — module context handles it.
            foreach (var migrator in moduleMigrators)
            {
                try
                {
                    logger.LogInformation("Applying {Module} migrations (core + module tables)...", migrator.ModuleName);
                    await migrator.MigrateAsync();
                    logger.LogInformation("{Module} migrations applied.", migrator.ModuleName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error applying {Module} migrations.", migrator.ModuleName);
                    throw;
                }
            }

            // Seed permissions using ApplicationDbContext (it can query the tables
            // that the module migration just created — same physical database).
            var db = sp.GetRequiredService<ApplicationDbContext>();
            await SeedPermissionsAsync(db, logger);
        }
    }

    private static async Task SeedPermissionsAsync(ApplicationDbContext db, ILogger logger)
    {
        var existingCategoryIds = await db.PermissionCategories
            .Select(c => c.Id)
            .ToListAsync();

        var newCategories = PermissionSeedData.GetCategories()
            .Where(c => !existingCategoryIds.Contains(c.Id))
            .ToList();

        if (newCategories.Count > 0)
        {
            db.PermissionCategories.AddRange(newCategories);
            await db.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} permission categories.", newCategories.Count);
        }

        var existingKeys = await db.Permissions
            .Select(p => p.Key)
            .ToListAsync();

        var newPermissions = PermissionSeedData.GetPermissions()
            .Where(p => !existingKeys.Contains(p.Key))
            .ToList();

        if (newPermissions.Count > 0)
        {
            db.Permissions.AddRange(newPermissions);
            await db.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} permissions.", newPermissions.Count);
        }
    }
}
