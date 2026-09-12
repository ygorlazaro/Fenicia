using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Notification;

public record AddNotificationCommand(
    [Required] string Title,
    [Required] string Description,
    DateTime? Date,
    string? ImageUrl);