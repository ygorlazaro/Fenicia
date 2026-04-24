namespace Fenicia.Auth.Domains.Notification.Commands;

public record AddNotificationCommand(
    string Title,
    string Description,
    DateTime? Date,
    string? ImageUrl);
