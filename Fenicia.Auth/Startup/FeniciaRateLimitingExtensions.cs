using AspNetCoreRateLimit;

namespace Fenicia.Auth.Startup;

public static class FeniciaRateLimitingExtensions
{
    public static WebApplicationBuilder AddFeniciaRateLimiting(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddMemoryCache();
        builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
        builder.Services.AddInMemoryRateLimiting();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return builder;
    }
}