using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Notification;

public class AddNotificationCommand()
{
    public AddNotificationCommand(string title, string description, DateTime? date, string? imageUrl)
        : this()
    {
        Title = title;
        Description = description;
        Date = date;
        ImageUrl = imageUrl;
    }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public DateTime? Date { get; set; }

    [MaxLength(200)]
    public string? ImageUrl { get; set; }
}
