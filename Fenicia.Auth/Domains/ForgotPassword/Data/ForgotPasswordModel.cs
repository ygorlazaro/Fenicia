namespace Fenicia.Auth.Domains.ForgotPassword.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Common.Database;

using User.Data;

[Table(name: "forgotten_passwords")]
public class ForgotPasswordModel : BaseModel
{
    [Required(ErrorMessage = "User ID is required")]
    [Column(name: "user_id")]
    [Display(Name = "User ID")]
    public Guid UserId
    {
        get; set;
    }

    [Required(ErrorMessage = "Verification code is required")]
    [Column(name: "code")]
    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "Code must be between 6 and 100 characters")]
    [Display(Name = "Verification Code")]
    [DataType(DataType.Text)]
    public string Code { get; set; } = null!;

    [Required]
    [Column(name: "expiration_date")]
    [Display(Name = "Expiration Date")]
    [DataType(DataType.DateTime)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(value: 1);

    [Required]
    [Column(name: "is_active")]
    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(ForgotPasswordModel.UserId))]
    [Display(Name = "User")]
    public virtual UserModel User { get; set; } = null!;
}
