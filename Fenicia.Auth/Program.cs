using Fenicia.Auth.Startup;
using Fenicia.Common.Data.Contexts;

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
            .AddFeniciaControllers(configuration)
            .AddFeniciaDependencyInjection()
            .Start();
    }
}