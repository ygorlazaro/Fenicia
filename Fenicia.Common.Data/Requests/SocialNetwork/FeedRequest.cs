using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.SocialNetwork;

public class FeedRequest
{
    public DateTime Date
    {
        get;
        set;
    }

    [Required]
    [MaxLength(512)]
    public string Text
    {
        get;
        set;
    }

    public Guid Id
    {
        get;
        set;
    }

    public Guid UserId
    {
        get;
        set;
    }
}
