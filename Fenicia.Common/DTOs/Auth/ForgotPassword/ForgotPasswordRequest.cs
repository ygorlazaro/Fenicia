using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.ForgotPassword;

/// <summary>
/// Request DTO for initiating a forgot password process.
/// </summary>
public class ForgotPasswordRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordRequest"/> class with the specified details.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="ipAddress">The IP address of the request (optional).</param>
    /// <param name="userAgent">The user agent string of the request (optional).</param>
    public ForgotPasswordRequest(string email, Guid userId, string? ipAddress = null, string? userAgent = null)
        : this()
    {
        Email = email;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        UserId = userId;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [Required]
    [MaxLength(48)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the IP address of the request.
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent string of the request.
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }
}
