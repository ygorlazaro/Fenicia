using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Notification;

/// <summary>
/// Request DTO for creating or updating a notification.
/// </summary>
public class NotificationRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationRequest"/> class with the specified notification details.
    /// </summary>
    /// <param name="id">The unique identifier of the notification.</param>
    /// <param name="title">The title of the notification.</param>
    /// <param name="description">The description of the notification.</param>
    /// <param name="date">The date of the notification.</param>
    /// <param name="imageUrl">The optional image URL for the notification.</param>
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

    /// <summary>
    /// Gets or sets the unique identifier of the notification.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the notification.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the notification.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the notification.
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the optional image URL for the notification.
    /// </summary>
    [MaxLength(200)]
    public string? ImageUrl { get; set; }
}
