using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fenicia.Common.API.Startup;

public static class FeniciaDatabaseExtensions
{
    public static WebApplicationBuilder AddFeniciaDbContext<TContext>(
        this WebApplicationBuilder builder,
        IConfiguration configuration,
        string migrationAssembly,
        string connectionStringName)
        where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string inválida");
        }

        builder.Services.AddDbContext<TContext>(o =>
            o.UseNpgsql(connectionString, b => b.MigrationsAssembly(migrationAssembly)).EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).UseSnakeCaseNamingConvention());

        builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<TContext>());

        return builder;
    }
}