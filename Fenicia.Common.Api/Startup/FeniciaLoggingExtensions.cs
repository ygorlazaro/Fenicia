using Microsoft.AspNetCore.Builder;

using Serilog;

namespace Fenicia.Common.API.Startup;

public static class FeniciaLoggingExtensions
{
    public static WebApplicationBuilder AddFeniciaLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);

            var seqUrl = context.Configuration["Seq:Url"];

            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                config.Enrich.FromLogContext().Enrich.WithEnvironmentUserName().WriteTo.Console().WriteTo.Seq(seqUrl);
            }
        });

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        return builder;
    }
}