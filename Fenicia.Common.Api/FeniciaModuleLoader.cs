using Fenicia.Common.API.Providers;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fenicia.Common.API;

public static class FeniciaModuleLoader
{
    public static string Load(string[] args, out ConfigurationManager configuration,
        out WebApplicationBuilder builder)
    {
        var tenantArg = args.FirstOrDefault(x => x.StartsWith("--tenant="));
        var tenantId = string.Empty;

        if (tenantArg is not null)
        {
            tenantId = tenantArg.Split("=")[1];

            Environment.SetEnvironmentVariable("TENANT_ID", tenantId);
        }

        configuration = new ConfigurationManager();
        var commonApiSettingsPath =
            Path.Combine(Directory.GetCurrentDirectory(), "../Fenicia.Common.Api/appsettings.json");

        if (!File.Exists(commonApiSettingsPath))
        {
            throw new FileNotFoundException($"Could not find shared appsettings.json at {commonApiSettingsPath}");
        }

        configuration.AddJsonFile(commonApiSettingsPath, false, true);

        builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configuration);
        builder.Services.AddScoped<TenantProvider>();

        return tenantId;
    }
}