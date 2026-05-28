using Flowtap_Configuration.DependencyInjection;
using Flowtap_Configuration.Logging;
using Flowtap_Medical.Extensions;
using Flowtap_Middleware;
using Flowtap_Presentation.Hubs;
using Serilog;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = SerilogConfiguration.CreateLogger(builder.Configuration);
builder.Host.UseSerilog();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);
builder.Services.AddMedicalModule(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try { await Flowtap_Infrastructure.Persistence.DatabaseSeeder.SeedAsync(services, logger); }
    catch (Exception ex) { logger.LogError(ex, "An error occurred during database migration/seeding."); }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flowtap Medical API v1"); c.RoutePrefix = "swagger"; });
}

app.UseCors("AllowAll");   // CORS must be before any middleware that can short-circuit
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
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "medical", timestamp = DateTime.UtcNow })).WithTags("Health");

try { Log.Information("Starting Flowtap Medical API"); await app.RunAsync(); }
catch (Exception ex) { Log.Fatal(ex, "Flowtap Medical API failed to start"); }
finally { await Log.CloseAndFlushAsync(); }
