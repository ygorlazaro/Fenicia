using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Company.DTOs;

public record GetCompaniesByUserResponse([Required] Guid Id, [Required][MaxLength(200)] string Name, [Required][MaxLength(200)] string Cnpj, [Required][MaxLength(200)] string Role);
