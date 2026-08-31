using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Notification.DTOs;

public record AddNotificationCommand(
    [Required] string Title,
    [Required] string Description,
    DateTime? Date,
    string? ImageUrl);
