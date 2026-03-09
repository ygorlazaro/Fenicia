using System.Text.Json.Serialization;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Token;

public class UserResponse
{
    public UserResponse()
    {
        this.Id = Guid.Empty;
        this.Name = string.Empty;
        this.Email = string.Empty;
    }

    public UserResponse(UserModel model)
    {
        this.Id = model.Id;
        this.Name = model.Name;
        this.Email = model.Email;

    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}
