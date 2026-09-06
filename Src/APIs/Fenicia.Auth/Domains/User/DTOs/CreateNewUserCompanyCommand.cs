using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Validations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record CreateNewUserCompanyCommand(
    [Required] [MaxLength(200)] [Cnpj] string Cnpj,
    [Required] [MaxLength(200)] string Name);
