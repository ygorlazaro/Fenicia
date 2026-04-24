namespace Fenicia.Auth.Domains.Notification.Commands;

public record UpdateNotificationCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime? Date,
    string? ImageUrl,
    bool? Read);
