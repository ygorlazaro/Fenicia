using Fenicia.Auth.Domains.Company.CheckCompanyExists;
using Fenicia.Auth.Domains.LoginAttempt.IncrementAttempts;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Migrations.Services;
using Fenicia.Externals.Email;

namespace Fenicia.Auth;

public static class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationManager();
        var commonApiSettingsPath =
            Path.Combine(Directory.GetCurrentDirectory(), "../Fenicia.Common.Api/appsettings.json");

        if (!File.Exists(commonApiSettingsPath))
        {
            throw new FileNotFoundException($"Could not find shared appsettings.json at {commonApiSettingsPath}");
        }

        configuration.AddJsonFile(commonApiSettingsPath, false, true);

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configuration);

        builder.AddFeniciaLogging()
            .AddFeniciaDbContext<AuthContext>(configuration, "Fenicia.Auth", "Auth")
            .AddFeniciaRateLimiting(configuration)
            .AddFeniciaCors()
            .AddFeniciaAuthentication(configuration)
            .AddFeniciaControllers()
            .AddFeniciaDependencyInjection(() =>
            {
                builder.Services.AddTransient<IMigrationService, MigrationService>();
                builder.Services.AddTransient<IBrevoProvider, BrevoProvider>();

                builder.Services.AddScoped<IncrementAttempts>();
            });
            

        builder.Start();
    }
}