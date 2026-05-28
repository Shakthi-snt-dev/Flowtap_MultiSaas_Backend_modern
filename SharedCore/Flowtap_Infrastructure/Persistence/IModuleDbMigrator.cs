namespace Flowtap_Infrastructure.Persistence;

/// <summary>
/// Implemented by each industry module to expose its DbContext for migration.
/// DatabaseSeeder resolves all IModuleDbMigrator registrations at startup
/// and calls MigrateAsync() on each — creating module-specific tables
/// that are NOT part of the core ApplicationDbContext schema.
/// </summary>
public interface IModuleDbMigrator
{
    /// <summary>
    /// Applies pending migrations for this module's DbContext.
    /// Safe to call even if migrations are already applied (idempotent).
    /// </summary>
    Task MigrateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Human-readable name used in log output. E.g. "Food", "Repair".
    /// </summary>
    string ModuleName { get; }
}
