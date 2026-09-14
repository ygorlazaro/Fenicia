using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Configuration;

public record UpsertConfigurationCommand(
    Guid? Id,
    [Required] Guid UserId,
    [Required] ConfigType ConfigType,
    [Required] string Value);