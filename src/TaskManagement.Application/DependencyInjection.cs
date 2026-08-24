using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Mapster;
using MediatR;
using System.Reflection;

namespace TaskManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(Common.Behaviors.ValidationBehavior<,>));
        });
        
        // Mapster
        TypeAdapterConfig.GlobalSettings.Scan(assembly);

        return services;
    }
}
