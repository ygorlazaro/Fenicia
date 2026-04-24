using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("notifications", Schema = "auth")]
public class NotificationModel : BaseCompanyModel
{
    [Column("title")]
    public string Title { get; set; } = null!;

    [Column("description")]
    public string Description { get; set; } = null!;

    [Column("date")]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("read")]
    public bool Read { get; set; } = false;
}
