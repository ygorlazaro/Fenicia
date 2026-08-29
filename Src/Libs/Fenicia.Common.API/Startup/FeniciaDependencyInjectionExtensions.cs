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

    private static void RegisterAllHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Could not determine the entry assembly for handler registration.");

        var handlerTypes = assembly.GetTypes().Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true } && (t.Name.EndsWith("Handler", StringComparison.Ordinal) || t.Name.EndsWith("Service", StringComparison.Ordinal)));

        foreach (var handlerType in handlerTypes)
        {
            services.AddTransient(handlerType);
        }
    }
}
