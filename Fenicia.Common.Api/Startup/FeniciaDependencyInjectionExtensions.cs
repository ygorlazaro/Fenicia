using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

namespace Fenicia.Common.API.Startup;

public static class FeniciaDependencyInjectionExtensions
{
    public static WebApplicationBuilder AddFeniciaDependencyInjection(this WebApplicationBuilder builder)
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

        return builder;
    }
}