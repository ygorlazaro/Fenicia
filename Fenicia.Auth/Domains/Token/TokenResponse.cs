using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Fenicia.Auth.Domains.Token;

[DataContract]
public class TokenResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = null!;

    public UserResponse User { get; set; } = null!;
}