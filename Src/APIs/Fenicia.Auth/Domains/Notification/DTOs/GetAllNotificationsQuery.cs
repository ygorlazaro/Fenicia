namespace Fenicia.Auth.Domains.Notification.DTOs;

public record GetAllNotificationsQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);
