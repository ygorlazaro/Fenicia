namespace Fenicia.Common.Api;

using Microsoft.Extensions.Configuration;

public static class AppSettingsReader
{
    public static string? GetConnectionString(string connectionStringName)
    {
        var configuration = AppSettingsReader.GetConfiguration();
        return configuration.GetConnectionString(connectionStringName);
    }

    public static ConfigurationManager GetConfiguration()
    {
        //var assemblyLocation = typeof(AppSettingsReader).Assembly.Location;
        var directoryName = Path.Combine(Directory.GetCurrentDirectory(), "../Fenicia.Common.Api");

        var config = new ConfigurationManager();
        config
            .SetBasePath(directoryName)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        return config;
    }
}
