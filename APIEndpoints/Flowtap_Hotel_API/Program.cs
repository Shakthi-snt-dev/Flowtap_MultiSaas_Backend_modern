using Flowtap_Configuration.DependencyInjection;
using Flowtap_Configuration.Logging;
using Flowtap_Hotel.Extensions;
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
builder.Services.AddHotelModule(builder.Configuration);

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
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flowtap Hotel API v1"); c.RoutePrefix = "swagger"; });
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
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "hotel", timestamp = DateTime.UtcNow })).WithTags("Health");

try { Log.Information("Starting Flowtap Hotel API"); await app.RunAsync(); }
catch (Exception ex) { Log.Fatal(ex, "Flowtap Hotel API failed to start"); }
finally { await Log.CloseAndFlushAsync(); }
