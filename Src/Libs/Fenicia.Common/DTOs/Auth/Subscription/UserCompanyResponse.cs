using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Subscription;

public record UserCompanyResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Cnpj);