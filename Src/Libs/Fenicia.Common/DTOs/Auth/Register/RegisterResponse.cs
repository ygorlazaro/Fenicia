using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Common.DTOs.Auth.Register;

public record RegisterResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Email,
    CreateNewUserCompanyResponse Company);