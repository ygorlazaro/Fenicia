using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Externals.Email;

using MediatR;

namespace Fenicia.Auth;

public static class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationManager();
        var commonApiSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "../Fenicia.Common.Api/appsettings.json");

        if (!File.Exists(commonApiSettingsPath))
        {
            throw new FileNotFoundException($"Could not find shared appsettings.json at {commonApiSettingsPath}");
        }

        configuration.AddJsonFile(commonApiSettingsPath, false, true);

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configuration);

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors().AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaLocalization().AddFeniciaDependencyInjection(() =>
        {
            builder.Services.AddTransient<IBrevoProvider, BrevoProvider>();
            builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
            builder.Services.AddHttpContextAccessor();
        }).AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth");


        builder.Start();

        var app = builder.Build();
        app.UseFeniciaLocalization();
    }
}
