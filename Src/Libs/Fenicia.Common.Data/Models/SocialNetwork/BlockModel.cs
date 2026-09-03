using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("blocks", Schema = "social_network")]
public class BlockModel : BaseModel
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid BlockedUserId { get; init; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; init; } = default!;

    [ForeignKey(nameof(BlockedUserId))]
    public UserModel BlockedUser { get; init; } = default!;

    [MaxLength(256)]
    public string? Reason { get; set; }

    public DateTime BlockDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}