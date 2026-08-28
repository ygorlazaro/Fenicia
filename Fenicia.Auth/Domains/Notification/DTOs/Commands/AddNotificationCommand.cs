namespace Fenicia.Auth.Domains.Notification.DTOs.Commands;

public record AddNotificationCommand(
    string Title,
    string Description,
    DateTime? Date,
    string? ImageUrl);
