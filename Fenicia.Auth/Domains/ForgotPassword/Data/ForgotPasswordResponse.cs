namespace Fenicia.Auth.Domains.ForgotPassword.Data;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

[Serializable]
public class ForgotPasswordResponse
{
    [Required(ErrorMessage = "Request ID is required")]
    [Display(Name = "Request ID", Description = "Unique identifier for the password reset request")]
    [JsonPropertyName(name: "requestId")]
    public Guid Id
    {
        get; set;
    }

    public static ForgotPasswordResponse Convert(ForgotPasswordModel model)
    {
        return new ForgotPasswordResponse
        {
            Id = model.Id
        };
    }
}
