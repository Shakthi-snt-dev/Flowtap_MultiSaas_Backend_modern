using Flowtap_Configuration.DependencyInjection;
using Flowtap_Configuration.Logging;
using Flowtap_Food.DbContext;
using Flowtap_Hotel.DbContext;
using Flowtap_Infrastructure.Persistence;
using Flowtap_Infrastructure.Persistence.Interceptors;
using Flowtap_Jewelry.DbContext;
using Flowtap_Medical.DbContext;
using Flowtap_Middleware;
using Flowtap_Presentation.Hubs;
using Flowtap_Presentation.Persistence;
using Flowtap_Repair.DbContext;
using Flowtap_Food.Extensions;
using Flowtap_Hotel.Extensions;
using Flowtap_Jewelry.Extensions;
using Flowtap_Medical.Extensions;
using Flowtap_Repair.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// ─── Logging ─────────────────────────────────────────────────────────────────
Log.Logger = SerilogConfiguration.CreateLogger(builder.Configuration);
builder.Host.UseSerilog();

// ─── Core Services ────────────────────────────────────────────────────────────
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);

// ─── Industry Modules ─────────────────────────────────────────────────────────
// These register each module's handlers, controllers, and seeders.
// Their DbContext + IModuleDbMigrator registrations will be REPLACED below
// by PresentationDbContext, which handles the full schema in one migration.
builder.Services.AddRepairModule(builder.Configuration);
builder.Services.AddFoodModule(builder.Configuration);
builder.Services.AddHotelModule(builder.Configuration);
builder.Services.AddMedicalModule(builder.Configuration);
builder.Services.AddJewelryModule(builder.Configuration);

// ─── PresentationDbContext ────────────────────────────────────────────────────
// Flowtap_Presentation has its own dedicated database that must contain ALL tables
// (core + food + repair + hotel + medical + jewelry) in one schema.
//
// PresentationDbContext implements all 5 module interfaces and inherits
// ApplicationDbContext — its single migration creates the complete schema.
//
// We replace all 5 individual module DbContext + IModuleDbMigrator registrations
// with PresentationDbContext so that:
//   - IFoodDbContext   → PresentationDbContext
//   - IRepairDbContext → PresentationDbContext
//   - IHotelDbContext  → PresentationDbContext
//   - IMedicalDbContext→ PresentationDbContext
//   - IJewelryDbContext→ PresentationDbContext
//   - IModuleDbMigrator→ one entry (PresentationDbContext migration)

builder.Services.AddDbContext<PresentationDbContext>((sp, options) =>
    options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            npgsql => npgsql.MigrationsAssembly("Flowtap_Presentation"))
        .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

// Override all module interface resolutions to use PresentationDbContext
builder.Services.AddScoped<IFoodDbContext>(sp   => sp.GetRequiredService<PresentationDbContext>());
builder.Services.AddScoped<IRepairDbContext>(sp  => sp.GetRequiredService<PresentationDbContext>());
builder.Services.AddScoped<IHotelDbContext>(sp   => sp.GetRequiredService<PresentationDbContext>());
builder.Services.AddScoped<IMedicalDbContext>(sp => sp.GetRequiredService<PresentationDbContext>());
builder.Services.AddScoped<IJewelryDbContext>(sp => sp.GetRequiredService<PresentationDbContext>());

// Replace the 5 individual IModuleDbMigrator entries with one Presentation migrator.
// DatabaseSeeder sees 1 migrator → runs PresentationDbContext.MigrateAsync() → full schema.
builder.Services.RemoveAll<IModuleDbMigrator>();
builder.Services.AddScoped<IModuleDbMigrator>(sp =>
    new ModuleDbMigrator<PresentationDbContext>(
        sp.GetRequiredService<PresentationDbContext>(), "Presentation"));

var app = builder.Build();

// ─── Seed Database ────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        await Flowtap_Infrastructure.Persistence.DatabaseSeeder.SeedAsync(services, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database migration/seeding.");
    }
}

// ─── Middleware pipeline ──────────────────────────────────────────────────────
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flowtap API v1");
        c.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();   // skip in dev — Swagger uses HTTP and redirect strips Authorization header
app.UseRateLimiter();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantMiddleware>();
app.UseHangfireDashboard();

app.MapControllers();
app.MapHub<CommunicationsHub>("/hubs/communications");

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithTags("Health");

try
{
    Log.Information("Starting Flowtap API");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Flowtap API failed to start");
}
finally
{
    await Log.CloseAndFlushAsync();
}
