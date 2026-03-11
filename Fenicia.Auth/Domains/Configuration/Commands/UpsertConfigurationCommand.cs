using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Configuration.Commands;

public record UpsertConfigurationCommand(
    [Required] Guid UserId,
    [Required] ConfigType ConfigType,
    [Required] string Value,
    Guid CompanyId
);
