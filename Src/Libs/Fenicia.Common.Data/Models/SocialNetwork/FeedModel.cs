using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

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
    public Guid UserId { get; init; }

    [ForeignKey(nameof(UserId))]
    public UserModel UserModel { get; init; } = default!;

    public List<CommentModel> Comments { get; init; } = [];

    public List<LikeModel> Likes { get; init; } = [];

    public List<ShareModel> Shares { get; init; } = [];
}