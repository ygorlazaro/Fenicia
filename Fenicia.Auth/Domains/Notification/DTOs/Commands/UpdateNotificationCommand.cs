namespace Fenicia.Auth.Domains.Notification.DTOs.Commands;

public record UpdateNotificationCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime? Date,
    string? ImageUrl,
    bool? Read);
