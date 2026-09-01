using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fenicia.Common.API.Startup;

public static class FeniciaCorsExtensions
{
    public static WebApplicationBuilder AddFeniciaCors(this WebApplicationBuilder builder)
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
                             ["http://localhost:5173"];

        builder.Services.AddCors(o =>
        {
            o.AddPolicy("RestrictedCors", policy => { policy.WithOrigins("https://fenicia.gatoninja.com.br", "https://api.fenicia.gatoninja.com.br").AllowAnyHeader().AllowAnyMethod().AllowCredentials(); });
            o.AddPolicy("DevCors", policy => { policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials(); });
        });

        return builder;
    }
}
