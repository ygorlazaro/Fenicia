using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public class LikeCommand()
{
    public LikeCommand(Guid feedId)
        : this()
    {
        FeedId = feedId;
    }

    [Required]
    public Guid FeedId { get; set; }
}
