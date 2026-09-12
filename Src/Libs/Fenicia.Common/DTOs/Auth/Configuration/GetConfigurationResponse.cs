using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Configuration;

public record GetConfigurationResponse(
    [Required] Guid Id,
    [Required] Guid UserId,
    [Required] Guid CompanyId,
    [Required] ConfigType ConfigType,
    [Required] [MaxLength(200)] string Value);