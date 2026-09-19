using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.ForgotPassword;

public class ForgotPasswordRequest()
{
    public ForgotPasswordRequest(string email, Guid userId, string? ipAddress = null, string? userAgent = null)
        : this()
    {
        Email = email;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        UserId = userId;
    }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(48)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }
}
