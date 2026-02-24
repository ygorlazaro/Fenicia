namespace Fenicia.Auth.Startup;

public static class FeniciaCorsExtensions
{
    public static WebApplicationBuilder AddFeniciaCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(o =>
        {
            o.AddPolicy("RestrictedCors",
                policy =>
                {
                    policy.WithOrigins("https://fenicia.gatoninja.com.br", "https://api.fenicia.gatoninja.com.br")
                        .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });

            o.AddPolicy("DevCors",
                policy =>
                {
                    policy.WithOrigins("http://localhost:5144", "http://localhost:3000", "http://localhost:5144",
                        "http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
        });

        return builder;
    }
}