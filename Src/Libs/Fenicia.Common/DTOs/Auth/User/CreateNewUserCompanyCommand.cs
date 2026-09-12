using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record CreateNewUserCompanyCommand(
    [Required][MaxLength(200)] string Cnpj,
    [Required] [MaxLength(200)] string Name);
