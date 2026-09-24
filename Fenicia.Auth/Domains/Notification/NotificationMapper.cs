using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Notification;

namespace Fenicia.Auth.Domains.Notification;

/// <summary>
/// Provides mapping methods for notifications.
/// </summary>
public static class NotificationMapper
{

    /// <summary>
    /// Maps a notification model to a notification response.
    /// </summary>
    /// <param name="notification">The notification model to map.</param>
    /// <returns>The mapped notification response.</returns>
    public static NotificationResponse MapNotificationResponse(NotificationModel notification)
    {
        return new NotificationResponse(
            notification.Id,
            notification.Title,
            notification.Description,
            notification.Date,
            notification.ImageUrl);
    }
}
