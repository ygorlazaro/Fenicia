namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Database;

[Table("forgotten_passwords")]
public class ForgotPasswordModel : BaseModel
{
    [Required]
    [Column("user_id")]
    public Guid UserId
    {
        get; set;
    }

    [Required]
    [Column("code")]
    [StringLength(maximumLength: 100, MinimumLength = 6)]
    [DataType(DataType.Text)]
    public string Code { get; set; } = null!;

    [Required]
    [Column("expiration_date")]
    [DataType(DataType.DateTime)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(value: 1);

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(ForgotPasswordModel.UserId))]
    public virtual UserModel User { get; set; } = null!;
}
