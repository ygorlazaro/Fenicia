using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Feed;

public class UpdateFeedCommand()
{
    public UpdateFeedCommand(Guid id, DateTime date, string text)
        : this()
    {
        Id = id;
        Date = date;
        Text = text;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(200)]
    public string Text { get; set; } = string.Empty;
}
