using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

namespace Fenicia.Common.API.Startup;

public static class FeniciaDependencyInjectionExtensions
{
    public static WebApplicationBuilder AddFeniciaDependencyInjection(this WebApplicationBuilder builder, Action relatedDependencies)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var config = ConfigurationOptions.Parse("localhost", true);

            config.ConnectRetry = 3;
            config.ConnectTimeout = 5000;

            return ConnectionMultiplexer.Connect(config);
        });

        builder.Services.AddScoped<WideEventContext>();
        builder.Services.AddResponseCompression(o => { o.EnableForHttps = true; });

        relatedDependencies();

        builder.Services.RegisterAllHandlers();

        return builder;
    }

    /// <summary>
    /// Registers all public non-abstract classes ending with "Handler" from the entry assembly as Transient.
    /// Call this after registering all dependencies to ensure handler dependencies are resolved.
    /// </summary>
    private static void RegisterAllHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.GetEntryAssembly();

        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true }
                        && t.Name.EndsWith("Handler", StringComparison.Ordinal));

        foreach (var handlerType in handlerTypes)
        {
            services.AddTransient(handlerType);
        }
    }
}