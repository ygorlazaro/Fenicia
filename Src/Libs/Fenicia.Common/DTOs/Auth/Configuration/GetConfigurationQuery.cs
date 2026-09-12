using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Configuration;

public record GetConfigurationQuery(
    [Required] Guid UserId,
    [Required] Guid CompanyId);