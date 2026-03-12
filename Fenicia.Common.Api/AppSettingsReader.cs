using Microsoft.Extensions.Configuration;

namespace Fenicia.Common.API;

public static class AppSettingsReader
{
    public static ConfigurationManager GetConfiguration()
    {
        var config = new ConfigurationManager();
        var solutionDir = GetSolutionDirectory();

        ArgumentNullException.ThrowIfNull(solutionDir);

        var possibleDirs = new List<string> { Path.Combine(solutionDir,
            "Fenicia.Common.Api") };
        var foundDir = possibleDirs.FirstOrDefault(dir => File.Exists(Path.Combine(dir,
                           "appsettings.json")))
                       ?? throw new FileNotFoundException(
                           $"Could not find appsettings.json in any known location. Checked: {string.Join(", ", possibleDirs.Select(d => Path.Combine(d, "appsettings.json")))}");

        // disable reloadOnChange in tests to avoid creating many FileSystemWatchers and exhausting inotify limits
        config.SetBasePath(foundDir).AddJsonFile("appsettings.json",
            false,
            false);

        return config;
    }

    private static string? GetSolutionDirectory()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (dir != null && dir.GetFiles("*.sln").Length == 0)
        {
            dir = dir.Parent;
        }

        return dir?.FullName;
    }
}
