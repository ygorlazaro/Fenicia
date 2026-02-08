using System.Runtime.Serialization;

namespace Fenicia.Common.Data.Responses.Auth;

[DataContract]
public class TokenResponse
{
    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public UserResponse User { get; set; } = null!;
}