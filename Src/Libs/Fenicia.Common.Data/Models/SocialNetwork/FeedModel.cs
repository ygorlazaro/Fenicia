using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetworkModels;

[Table("feeds", Schema = "social_network")]
public class FeedModel : BaseCompanyModel
{
    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(512)]
    public string Text { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserModel UserModel { get; set; } = null!;

    public List<CommentModel> Comments { get; set; } = [];

    public List<LikeModel> Likes { get; set; } = [];

    public List<ShareModel> Shares { get; set; } = [];
}
