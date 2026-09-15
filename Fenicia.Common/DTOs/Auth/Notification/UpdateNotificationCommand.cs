using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Notification;

public class UpdateNotificationCommand()
{
    public UpdateNotificationCommand(
        Guid id,
        string title,
        string description,
        DateTime? date,
        string? imageUrl,
        bool? isRead)
        : this()
    {
        Id = id;
        Title = title;
        Description = description;
        Date = date;
        ImageUrl = imageUrl;
        IsRead = isRead;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public DateTime? Date { get; set; }

    [MaxLength(200)]
    public string? ImageUrl { get; set; }

    public bool? IsRead { get; set; }
}
