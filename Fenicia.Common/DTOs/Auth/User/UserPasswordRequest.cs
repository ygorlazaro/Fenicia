using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public class UserPasswordRequest()
{
    public UserPasswordRequest(Guid userId, string? currentPassword, string newPassword, string confirmPassword)
        : this()
    {
        UserId = userId;
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
        ConfirmPassword = confirmPassword;
    }

    [Required]
    public Guid UserId { get; set; }

    public string? CurrentPassword { get; set; }

    [Required]
    [MaxLength(200)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
