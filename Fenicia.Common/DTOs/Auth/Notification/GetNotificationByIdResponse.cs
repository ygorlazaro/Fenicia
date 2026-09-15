using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Notification;

public class GetNotificationByIdResponse()
{
    public GetNotificationByIdResponse(
        Guid id,
        string title,
        string description,
        DateTime date,
        string? imageUrl,
        bool read)
        : this()
    {
        Id = id;
        Title = title;
        Description = description;
        Date = date;
        ImageUrl = imageUrl;
        Read = read;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(200)]
    public string? ImageUrl { get; set; }

    [Required]
    public bool Read { get; set; }
}
