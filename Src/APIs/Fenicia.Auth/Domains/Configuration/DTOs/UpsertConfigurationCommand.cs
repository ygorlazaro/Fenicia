using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Configuration.DTOs;

public record UpsertConfigurationCommand(
    Guid? Id,
    [Required] Guid UserId,
    [Required] ConfigType ConfigType,
    [Required] string Value);