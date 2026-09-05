using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("blocks", Schema = "social_network")]
public class BlockModel : BaseModel
{
    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid BlockedProfileId { get; init; }

    [ForeignKey(nameof(ProfileId))]
    public ProfileModel Profile { get; init; } = default!;

    [ForeignKey(nameof(BlockedProfileId))]
    public ProfileModel BlockedProfile { get; init; } = default!;

    [MaxLength(256)]
    public string? Reason { get; set; }

    public DateTime BlockDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
