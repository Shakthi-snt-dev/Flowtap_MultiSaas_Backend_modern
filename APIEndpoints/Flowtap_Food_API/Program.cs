using Flowtap_Configuration.DependencyInjection;
using Flowtap_Configuration.Logging;
using Flowtap_Food.Extensions;
using Flowtap_Middleware;
using Flowtap_Presentation.Hubs;
using Serilog;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// ─── Logging ─────────────────────────────────────────────────────────────────
Log.Logger = SerilogConfiguration.CreateLogger(builder.Configuration);
builder.Host.UseSerilog();

// ─── Services ────────────────────────────────────────────────────────────────
builder.Services.AddApplicationServices();          // core: auth, inventory, sales, purchasing
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);
builder.Services.AddFoodModule(builder.Configuration);  // food: tables, KOT, recipes, raw materials

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flowtap Food API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowAll");
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

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "food", timestamp = DateTime.UtcNow }))
   .WithTags("Health");

try
{
    Log.Information("Starting Flowtap Food API");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Flowtap Food API failed to start");
}
finally
{
    await Log.CloseAndFlushAsync();
}
