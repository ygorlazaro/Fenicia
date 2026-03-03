using Fenicia.Auth.Domains.LoginAttempt.IncrementAttempts;
using Fenicia.Auth.Domains.Subscription.GetUserProfile;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
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
            .AddFeniciaRateLimiting(configuration)
            .AddFeniciaCors()
            .AddFeniciaAuthentication(configuration)
            .AddFeniciaControllers()
            .AddFeniciaDependencyInjection(() =>
            {
                builder.Services.AddTransient<IBrevoProvider, BrevoProvider>();
                builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
                builder.Services.AddHttpContextAccessor();

                builder.Services.AddScoped<IncrementAttempts>();
                builder.Services.AddScoped<GetUserProfileHandler>();
            })
            .AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth");


        builder.Start();
    }
}
