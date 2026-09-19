using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Notification;

public class NotificationRequest()
{
    public NotificationRequest(
        Guid id,
        string title,
        string description,
        DateTime date,
        string? imageUrl)
        : this()
    {
        Id = id;
        Title = title;
        Description = description;
        Date = date;
        ImageUrl = imageUrl;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(200)]
    public string? ImageUrl { get; set; }

    public bool? IsRead { get; set; }
}
