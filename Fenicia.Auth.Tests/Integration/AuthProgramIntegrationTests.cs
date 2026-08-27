using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Fenicia.Common.API.Startup;

namespace Fenicia.Auth.Tests.Integration;

public class AuthProgramIntegrationTests
{
    [Fact]
    public void Program_StartupBuildsSuccessfully()
    {
        var args = Array.Empty<string>();
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
            builder.Services.AddTransient<Externals.Email.IBrevoProvider, TestBrevoProvider>();
            builder.Services.AddSingleton<Common.Data.ICompanyContext, Common.Data.CompanyContext>();
            builder.Services.AddHttpContextAccessor();
        }).AddFeniciaDbContext<Common.Data.Contexts.DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        var app = builder.Build();
        app.UseFeniciaLocalization();

        Assert.NotNull(app);
    }

    private class TestBrevoProvider : Externals.Email.IBrevoProvider
    {
        public void Send(Common.Enums.External.EmailTemplate template, string email, string name, Dictionary<string, object>? parameters)
        {
        }
    }
}
