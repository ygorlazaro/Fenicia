namespace Fenicia.Auth.Domains.Notification.DTOs;

public record AddNotificationCommand(
    string Title,
    string Description,
    DateTime? Date,
    string? ImageUrl);
