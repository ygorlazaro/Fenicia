using System.ComponentModel.DataAnnotations;
using Fenicia.Auth.Domains.User.DTOs;

namespace Fenicia.Auth.Domains.Register.DTOs;

public record RegisterResponse([Required] Guid Id, [Required][MaxLength(200)] string Name, [Required][MaxLength(200)] string Email, CreateNewUserCompanyResponse Company);
