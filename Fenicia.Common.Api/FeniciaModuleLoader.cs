using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Fenicia.Common.API;

public static class FeniciaModuleLoader
{
    public static string Load(string[] args, out ConfigurationManager configuration, out WebApplicationBuilder builder)
    {
        var tenantArg = args.FirstOrDefault(x => x.StartsWith("--tenant="));
        var tenantId = string.Empty;

        if (tenantArg is not null)
        {
            tenantId = tenantArg.Split("=")[1];

            Environment.SetEnvironmentVariable("TENANT_ID", tenantId);
        }

        configuration = new ConfigurationManager();
        var commonApiSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Common.json");

        if (!File.Exists(commonApiSettingsPath))
        {
            throw new FileNotFoundException($"Could not find shared appsettings.json at {commonApiSettingsPath}");
        }

        configuration.AddJsonFile(commonApiSettingsPath, false, true);
        configuration.AddEnvironmentVariables();

        builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configuration);

        return tenantId;
    }
}