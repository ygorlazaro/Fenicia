using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("notifications_history", Schema = "auth")]
public class NotificationHistoryModel : BaseCompanyModel
{
    [Required]
    public Guid NotificationId { get; set; }

    public Guid? UserId { get; set; }

    [Required]
    public bool IsRead { get; set; }

    public virtual UserModel? User { get; set; }

    public virtual CompanyModel Company { get; set; } = default!;

    public virtual NotificationModel Notification { get; set; } = default!;
}