using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record CreateNewUserCompanyCommand([Required][MaxLength(200)] string Cnpj, [Required][MaxLength(200)] string Name);
