using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("feeds", Schema = "social_network")]
public class FeedModel : BaseCompanyModel
{
    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(512)]
    public string Text { get; set; } = string.Empty;

    [Required]
    public Guid ProfileId { get; init; }

    [ForeignKey(nameof(ProfileId))]
    public ProfileModel Profile { get; init; } = default!;

    public List<CommentModel> Comments { get; init; } = [];

    public List<LikeModel> Likes { get; init; } = [];

    public List<ShareModel> Shares { get; init; } = [];
}
