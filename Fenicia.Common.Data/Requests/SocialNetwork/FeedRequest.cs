using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.SocialNetwork;

public class FeedRequest
{
    [Required]
    [MaxLength(512)]
    public string Text { get; set; } = string.Empty;

    public Guid Id { get; set; }

    public Guid UserId { get; set; }
}