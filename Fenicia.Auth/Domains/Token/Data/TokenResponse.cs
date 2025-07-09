namespace Fenicia.Auth.Domains.Token.Data;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Fenicia.Auth.Domains.User.Data;

[DataContract]
public class TokenResponse
{
    [Required]
    [StringLength(maximumLength: 2048)]
    [DataMember(Name = "token")]
    public string AccessToken { get; set; } = null!;

    [Required]
    [StringLength(maximumLength: 2048)]
    [DataMember(Name = "refreshToken")]
    public string RefreshToken { get; set; } = null!;

    public UserResponse User
    {
        get;
        set;
    } = null!;
}
