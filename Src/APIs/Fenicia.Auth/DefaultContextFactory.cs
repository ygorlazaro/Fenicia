using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Fenicia.Auth;

public class DefaultContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Common.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Auth")
            ?? "Host=localhost;Database=FeniciaAuth;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("Fenicia.Auth"))
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention();

        return new DefaultContext(optionsBuilder.Options, new CompanyContext(new HttpContextAccessor()));
    }
}
