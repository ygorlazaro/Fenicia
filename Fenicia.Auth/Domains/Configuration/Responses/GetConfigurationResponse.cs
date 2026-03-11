using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Configuration.GetConfiguration;

public record GetConfigurationResponse(
    Guid Id,
    Guid UserId,
    Guid? CompanyId,
    ConfigType ConfigType,
    string Value
);
