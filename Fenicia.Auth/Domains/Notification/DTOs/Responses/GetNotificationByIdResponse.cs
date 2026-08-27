namespace Fenicia.Auth.Domains.Notification.DTOs.Responses;

public record GetNotificationByIdResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime Date,
    string? ImageUrl,
    bool Read);
