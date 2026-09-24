using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("forgotten_passwords", Schema = "auth")]
public sealed class ForgotPasswordModel : BaseModel
{
    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("code")]
    [StringLength(6)]
    [DataType(DataType.Text)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Column("expiration_date")]
    [DataType(DataType.DateTime)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(1);

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("ip_address")]
    [MaxLength(45)]
    public string? IpAddress { get; init; }

    [Column("user_agent")]
    [MaxLength(500)]
    public string? UserAgent { get; init; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; init; } = null!;
}
