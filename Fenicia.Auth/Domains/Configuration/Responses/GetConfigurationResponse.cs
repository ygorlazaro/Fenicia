using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Configuration.Responses;

public record GetConfigurationResponse(

    Guid Id,

    Guid UserId,

    Guid CompanyId,

    ConfigType ConfigType,

    string Value);
