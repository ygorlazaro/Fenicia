using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fenicia.Common.API.Startup;

public static class FeniciaDatabaseExtensions
{
    public static WebApplicationBuilder AddFeniciaDbContext<TContext>(this WebApplicationBuilder builder, IConfiguration configuration, string migrationAssembly, string connectionStringName, string? tenantId = null) where TContext:DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionStringName);
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Connection string inválida");
        }

        if (!string.IsNullOrEmpty(tenantId))
        {
            connectionString =  connectionString.Replace(tenantId, tenantId);
        }

        builder.Services.AddDbContextPool<TContext>(o => o
            .UseNpgsql(connectionString, b => b.MigrationsAssembly(migrationAssembly))
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention());

        return builder;
    }
}