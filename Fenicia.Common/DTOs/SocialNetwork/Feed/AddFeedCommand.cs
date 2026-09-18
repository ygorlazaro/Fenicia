using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Feed;

public class AddFeedCommand()
{
    public AddFeedCommand(
        Guid id,
        DateTime date,
        string text,
        Guid profileId,
        Guid? originalFeedId)
        : this()
    {
        Id = id;
        Date = date;
        Text = text;
        ProfileId = profileId;
        OriginalFeedId = originalFeedId;
    }

    public Guid Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(512)]
    public string Text { get; set; } = string.Empty;

    [Required]
    public Guid ProfileId { get; set; }

    public Guid? OriginalFeedId { get; set; }
}
