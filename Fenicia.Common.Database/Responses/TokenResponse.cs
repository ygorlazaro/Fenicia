namespace Fenicia.Common.Database.Responses;

using System.Runtime.Serialization;

[DataContract]
public class TokenResponse
{
    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public UserResponse User
    {
        get;
        set;
    }

= null!;
}
