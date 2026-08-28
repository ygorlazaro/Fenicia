using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Externals.Email;

namespace Fenicia.Auth;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationManager();
        var commonApiSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Common.json");

        if (!File.Exists(commonApiSettingsPath))
        {
            throw new FileNotFoundException($"Could not find shared appsettings.json at {commonApiSettingsPath}");
        }

        configuration.AddJsonFile(commonApiSettingsPath, false, true);
        configuration.AddEnvironmentVariables();

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configuration);

        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors().AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaLocalization().AddFeniciaDependencyInjection(() =>
    {
        builder.Services.AddTransient<IBrevoProvider, BrevoProvider>();
        builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<UserRepository>();
        builder.Services.AddScoped<UserRoleRepository>();
        builder.Services.AddScoped<RoleRepository>();
        builder.Services.AddScoped<CompanyRepository>();
        builder.Services.AddScoped<ConfigurationRepository>();
    }).AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        var app = builder.Build();
        app.UseFeniciaLocalization();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_TESTING") != "true")
        {
            app.Run();
        }
    }
}
