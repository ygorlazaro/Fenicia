using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Subscription.DTOs;

public record UserCompanyResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Cnpj);