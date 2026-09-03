using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Company.DTOs;

public record UpdateCompanyRequest([Required] [MaxLength(50)] string Name);