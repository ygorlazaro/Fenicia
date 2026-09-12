using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record UpdateUserPasswordResponse(bool Success, [Required] [MaxLength(200)] string Message);