using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fenicia.Auth.Domains.ForgotPassword.Data;

/// <summary>
/// Represents the response object for a forgot password request.
/// Contains the unique identifier of the generated password reset request.
/// This response is returned after successfully initiating a password reset process.
/// </summary>
/// <remarks>
/// This class is used in the password reset workflow to provide a tracking identifier
/// for the password reset request.
/// </remarks>
[Serializable]
public class ForgotPasswordResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the forgot password request.
    /// This ID can be used to track the password reset request.
    /// </summary>
    /// <remarks>
    /// This identifier is generated when a forgot password request is created
    /// and should be used in subsequent password reset operations.
    /// </remarks>
    [Required(ErrorMessage = "Request ID is required")]
    [Display(Name = "Request ID", Description = "Unique identifier for the password reset request")]
    [JsonPropertyName("requestId")]
    public Guid Id { get; set; }
}
