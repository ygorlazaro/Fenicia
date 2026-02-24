using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fenicia.Common.API.Startup;

public static class FeniciaDatabaseExtensions
{
    public static WebApplicationBuilder AddFeniciaDbContext<TContext>(this WebApplicationBuilder builder, IConfiguration configuration, string migrationAssembly, string connectionStringName) where TContext:DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionStringName);

        builder.Services.AddDbContextPool<TContext>(o => o
            .UseNpgsql(connectionString, b => b.MigrationsAssembly(migrationAssembly))
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention());

        return builder;
    }
}