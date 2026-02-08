using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Data.Requests.SocialNetwork;

namespace Fenicia.Common.Data.Models.SocialNetwork;

public class FeedModel(FeedRequest request) : BaseModel
{
    [Required]
    public DateTime Date { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(512)]
    public string Text { get; set; } = request.Text;

    public Guid UserId { get; set; } = request.UserId;

    public UserModel User { get; set; } = null!;
}