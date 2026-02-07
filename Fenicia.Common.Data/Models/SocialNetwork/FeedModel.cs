using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Models.SocialNetwork;

public class FeedModel : BaseModel
{
    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(512)]
    public string Text { get; set; }

    public Guid UserId { get; set; }

    public UserModel User { get; set; }
}