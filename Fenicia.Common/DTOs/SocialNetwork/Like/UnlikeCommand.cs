using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public class UnlikeCommand()
{
    public UnlikeCommand(Guid feedId)
        : this()
    {
        FeedId = feedId;
    }

    [Required]
    public Guid FeedId { get; set; }
}
