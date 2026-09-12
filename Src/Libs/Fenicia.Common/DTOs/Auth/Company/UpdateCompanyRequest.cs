using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Company;

public record UpdateCompanyRequest([Required] [MaxLength(50)] string Name);