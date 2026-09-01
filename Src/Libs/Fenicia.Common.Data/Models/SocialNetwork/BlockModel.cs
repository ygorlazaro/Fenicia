using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("blocks", Schema = "social_network")]
public class BlockModel : BaseModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid BlockedUserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;

    [ForeignKey(nameof(BlockedUserId))]
    public UserModel BlockedUser { get; set; } = null!;

    [MaxLength(256)]
    public string? Reason { get; set; }

    public DateTime BlockDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
