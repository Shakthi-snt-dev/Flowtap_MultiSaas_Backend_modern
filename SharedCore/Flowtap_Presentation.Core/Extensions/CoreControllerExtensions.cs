using Microsoft.Extensions.DependencyInjection;

namespace Flowtap_Presentation.Core.Extensions;

/// <summary>
/// Registers the shared core controllers (Auth, Product, Sale, Inventory, etc.)
/// from the Flowtap_Presentation.Core assembly into the MVC application-part list.
/// Call this from every industry API's Program.cs after AddPresentationServices().
/// </summary>
public static class CoreControllerExtensions
{
    public static IServiceCollection AddCoreControllers(this IServiceCollection services)
    {
        services.AddControllers()
                .AddApplicationPart(typeof(CoreControllerExtensions).Assembly);
        return services;
    }
}
