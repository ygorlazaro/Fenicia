using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("feeds", Schema = "social_network")]
public class SNFeed : BaseModel
{
    [Required]
    public DateTime Date { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(512)]
    public string Text { get; set; } = string.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public AuthUser User { get; set; } = null!;
}
