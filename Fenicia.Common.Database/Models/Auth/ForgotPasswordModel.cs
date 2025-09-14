namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Database;

[Table("forgotten_passwords")]
public class ForgotPasswordModel : BaseModel
{
    [Required(ErrorMessage = "User ID is required")]
    [Column("user_id")]
    [Display(Name = "User ID")]
    public Guid UserId
    {
        get; set;
    }

    [Required(ErrorMessage = "Verification code is required")]
    [Column("code")]
    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "Code must be between 6 and 100 characters")]
    [Display(Name = "Verification Code")]
    [DataType(DataType.Text)]
    public string Code { get; set; } = null!;

    [Required]
    [Column("expiration_date")]
    [Display(Name = "Expiration Date")]
    [DataType(DataType.DateTime)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(value: 1);

    [Required]
    [Column("is_active")]
    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(ForgotPasswordModel.UserId))]
    [Display(Name = "User")]
    public virtual UserModel User { get; set; } = null!;
}
