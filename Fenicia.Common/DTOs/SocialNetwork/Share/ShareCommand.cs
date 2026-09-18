using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Share;

public class ShareCommand()
{
    public ShareCommand(Guid id, Guid originalFeedId, string? text)
        : this()
    {
        Id = id;
        OriginalFeedId = originalFeedId;
        Text = text;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid OriginalFeedId { get; set; }

    [MaxLength(200)]
    public string? Text { get; set; }
}
