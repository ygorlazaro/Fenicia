using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Configuration;

namespace Fenicia.Auth.Domains.Configuration;

public static class ConfigurationMapper
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