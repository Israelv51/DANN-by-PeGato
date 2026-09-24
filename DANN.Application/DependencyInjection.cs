using Microsoft.Extensions.DependencyInjection;

namespace DANN.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        var handlerTypes = assembly
            .GetTypes()
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.Name.EndsWith(
                    "Handler",
                    StringComparison.Ordinal));

        foreach (var handlerType in handlerTypes)
        {
            services.AddScoped(
                handlerType,
                handlerType);
        }

        return services;
    }
}
