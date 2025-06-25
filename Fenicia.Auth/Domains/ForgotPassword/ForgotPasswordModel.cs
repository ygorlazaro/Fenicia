using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Auth.Domains.User;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.ForgotPassword;

[Table("forgotten_passwords")]
public class ForgotPasswordModel: BaseModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Code { get; set; } = null!;

    public DateTime ExpirationDate { get; set; } = DateTime.Now.AddDays(1);

    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(UserId))]
    public virtual UserModel User { get; set; } = null!;
}
