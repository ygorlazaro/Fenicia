using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Configuration;

namespace Fenicia.Auth.Domains.Configuration;

/// <summary>
/// Mapper class for mapping between ConfigurationModel and ConfigurationResponse.
/// </summary>
public static class ConfigurationMapper
{
    /// <summary>
    /// Maps a ConfigurationModel to a ConfigurationResponse.
    /// </summary>
    /// <param name="configuration">The configuration model.</param>
    /// <returns>The configuration response.</returns>
    public static ConfigurationResponse MapToConfigurationResponse(ConfigurationModel configuration)
    {
        return new ConfigurationResponse(
            configuration.Id,
            configuration.UserId,
            configuration.CompanyId,
            configuration.ConfigType,
            configuration.Value);
    }
}
