using Fenicia.Auth.Domains.Configuration.DTOs;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Configuration;

public static partial class ConfigurationMapper
{
    public static GetConfigurationResponse MapToGetConfigurationResponse(this ConfigurationModel configuration)
    {
        return new GetConfigurationResponse(
            configuration.Id,
            configuration.UserId,
            configuration.CompanyId,
            configuration.ConfigType,
            configuration.Value);
    }
}
