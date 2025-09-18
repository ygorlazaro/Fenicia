namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

[Table("refresh_tokens")]
public class RefreshTokenModel : BaseModel
{
    [Required]
    [MaxLength(256)]
    [Column("token")]
    public string Token { get; set; } = null!;

    [Required]
    [Column("expiration_date")]
    [DataType(DataType.DateTime)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(value: 7);

    [Required]
    [Column("user_id")]
    public Guid UserId
    {
        get; set;
    }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(RefreshTokenModel.UserId))]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;
}
