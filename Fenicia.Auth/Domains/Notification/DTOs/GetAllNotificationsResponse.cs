namespace Fenicia.Auth.Domains.Notification.DTOs;

public record GetAllNotificationsResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime Date,
    string? ImageUrl,
    bool Read);
