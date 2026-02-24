using Fenicia.Auth.Domains.Company.CheckCompanyExists;
using Fenicia.Common.API;
using Fenicia.Common.Migrations.Services;
using Fenicia.Externals.Email;

using StackExchange.Redis;

namespace Fenicia.Auth.Startup;

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

        builder.Services.AddTransient<CheckCompanyExistsHandler>();

        builder.Services.AddResponseCompression(o => { o.EnableForHttps = true; });

        builder.Services.AddTransient<IMigrationService, MigrationService>();
        builder.Services.AddTransient<IBrevoProvider, BrevoProvider>();

        return builder;
    }
}