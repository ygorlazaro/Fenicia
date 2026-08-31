using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record UpdateUserPasswordResponse(bool Success, [Required][MaxLength(200)] string Message);
