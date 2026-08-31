using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Notification.DTOs;

public record UpdateNotificationCommand(
    [Required] Guid Id,
    [Required][MaxLength(200)] string Title,
    [Required][MaxLength(200)] string Description,
    DateTime? Date,
    [MaxLength(200)] string? ImageUrl,
    bool? IsRead);
