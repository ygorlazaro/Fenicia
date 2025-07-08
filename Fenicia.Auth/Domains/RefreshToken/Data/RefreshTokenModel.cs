namespace Fenicia.Auth.Domains.RefreshToken.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using User.Data;

[Table(name: "refresh_tokens")]
public class RefreshTokenModel : BaseModel
{
    [Required(ErrorMessage = "Token is required")]
    [MaxLength(length: 256, ErrorMessage = "Token cannot exceed 256 characters")]
    [Column(name: "token")]
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "Expiration date is required")]
    [Column(name: "expiration_date")]
    [DataType(DataType.DateTime)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(value: 7);

    [Required(ErrorMessage = "User ID is required")]
    [Column(name: "user_id")]
    public Guid UserId
    {
        get; set;
    }

    [Required(ErrorMessage = "IsActive status is required")]
    [Column(name: "is_active")]
    public bool IsActive { get; set; } = true;

    [ForeignKey(name: "UserId")]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;
}
