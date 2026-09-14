using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.ForgotPassword;

public record AddForgotPasswordCommand
{
    public AddForgotPasswordCommand()
    {
    }

    public AddForgotPasswordCommand(string email, string? ipAddress = null, string? userAgent = null)
    {
        Email = email;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? IpAddress { get; set; }

    [MaxLength(200)]
    public string? UserAgent { get; set; }
}
