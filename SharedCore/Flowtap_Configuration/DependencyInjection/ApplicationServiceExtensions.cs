using System.Reflection;
using Flowtap_Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Flowtap_Configuration.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var applicationAssembly = Assembly.Load("Flowtap_Application");

        services.AddAutoMapper(applicationAssembly);

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        });

        return services;
    }
}
