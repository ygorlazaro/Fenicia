namespace Fenicia.Auth.Domains.Notification.Responses;

public record GetNotificationByIdResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime Date,
    string? ImageUrl,
    bool Read);
