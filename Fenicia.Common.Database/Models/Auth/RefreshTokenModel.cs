namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

[Table("refresh_tokens")]
public class RefreshTokenModel : BaseModel
{
    [Required(ErrorMessage = "Token is required")]
    [MaxLength(length: 256, ErrorMessage = "Token cannot exceed 256 characters")]
    [Column("token")]
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "Expiration date is required")]
    [Column("expiration_date")]
    [DataType(DataType.DateTime)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(value: 7);

    [Required(ErrorMessage = "User ID is required")]
    [Column("user_id")]
    public Guid UserId
    {
        get; set;
    }

    [Required(ErrorMessage = "IsActive status is required")]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;
}
