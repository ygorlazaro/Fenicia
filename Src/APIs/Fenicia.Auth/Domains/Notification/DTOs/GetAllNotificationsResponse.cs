using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Notification.DTOs;

public record GetAllNotificationsResponse(
    [Required] Guid Id,
    [Required][MaxLength(200)] string Title,
    [Required][MaxLength(200)] string Description,
    [Required] DateTime Date,
    [MaxLength(200)] string? ImageUrl,
    bool Read);
