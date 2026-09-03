using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Configuration.DTOs;

public record GetConfigurationQuery(
    [Required] Guid UserId,
    [Required] Guid CompanyId);