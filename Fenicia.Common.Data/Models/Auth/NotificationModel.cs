using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("notifications", Schema = "auth")]
public class NotificationModel : BaseModel
{
    [Column("title")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Column("date")]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Column("image_url")]
    [MaxLength(200)]
    public string? ImageUrl { get; set; }

    public List<NotificationHistoryModel> History { get; set; } = null!;
}
