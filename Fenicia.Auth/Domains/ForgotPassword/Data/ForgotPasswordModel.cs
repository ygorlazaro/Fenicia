namespace Fenicia.Auth.Domains.ForgotPassword.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Common.Database;

using User.Data;

/// <summary>
///     Represents a forgotten password record in the system that tracks password reset requests and their status.
/// </summary>
/// <remarks>
///     This model is used to manage password reset functionality, including verification codes and expiration dates.
///     The records are stored in the 'forgotten_passwords' table in the 'auth' schema.
/// </remarks>
[Table(name: "forgotten_passwords")]
public class ForgotPasswordModel : BaseModel
{
    /// <summary>
    ///     Gets or sets the unique identifier of the user requesting password reset.
    /// </summary>
    /// <remarks>
    ///     This is a required field that establishes a relationship with the User table.
    /// </remarks>
    [Required(ErrorMessage = "User ID is required")]
    [Column(name: "user_id")]
    [Display(Name = "User ID")]
    public Guid UserId { get; set; }

    /// <summary>
    ///     Gets or sets the unique verification code for password reset.
    /// </summary>
    /// <remarks>
    ///     The code must be between 6 and 100 characters in length.
    /// </remarks>
    [Required(ErrorMessage = "Verification code is required")]
    [Column(name: "code")]
    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "Code must be between 6 and 100 characters")]
    [Display(Name = "Verification Code")]
    [DataType(DataType.Text)]
    public string Code { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the expiration date of the password reset request.
    ///     Default value is set to 24 hours from creation.
    /// </summary>
    /// <remarks>
    ///     By default, the expiration date is set to 24 hours from the creation time.
    ///     The date is stored in UTC format.
    /// </remarks>
    [Required]
    [Column(name: "expiration_date")]
    [Display(Name = "Expiration Date")]
    [DataType(DataType.DateTime)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(value: 1);

    /// <summary>
    ///     Gets or sets whether the password reset request is still active.
    /// </summary>
    /// <remarks>
    ///     Defaults to true when created. Set to false when the reset request is completed or expired.
    /// </remarks>
    [Required]
    [Column(name: "is_active")]
    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    ///     Gets or sets the associated user model.
    ///     This is a navigation property for the foreign key relationship.
    /// </summary>
    /// <remarks>
    ///     This is a navigation property that represents the foreign key relationship with the User table.
    /// </remarks>
    [ForeignKey(nameof(ForgotPasswordModel.UserId))]
    [Display(Name = "User")]
    public virtual UserModel User { get; set; } = null!;
}
