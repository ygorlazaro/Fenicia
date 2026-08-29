using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetworkModels;

[Table("feeds", Schema = "social_network")]
public class FeedModel : BaseCompanyModel
{
    [Required]
    public DateTime Date { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(512)]
    public string Text { get; set; } = string.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public UserModel UserModel { get; set; } = null!;
}