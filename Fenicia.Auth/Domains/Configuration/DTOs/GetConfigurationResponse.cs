using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Configuration.DTOs;

public record GetConfigurationResponse(

    Guid Id,

    Guid UserId,

    Guid CompanyId,

    ConfigType ConfigType,

    string Value);
