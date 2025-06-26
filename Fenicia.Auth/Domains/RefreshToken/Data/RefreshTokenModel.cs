using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.Data;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.RefreshToken.Data;

[Table("refresh_tokens")]
public class RefreshTokenModel : BaseModel
{
    [Required]
    [MaxLength(256)]
    public string Token { get; set; } = null!;

    [Required]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(7);

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;


    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;
}
