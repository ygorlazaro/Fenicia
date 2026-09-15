using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public class UpdateUserPasswordResponse()
{
    public UpdateUserPasswordResponse(bool success, string message)
        : this()
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; set; }

    [Required]
    [MaxLength(200)]
    public string Message { get; set; } = string.Empty;
}
