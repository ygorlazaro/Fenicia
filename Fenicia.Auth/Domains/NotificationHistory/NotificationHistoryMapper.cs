using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Notification;

namespace Fenicia.Auth.Domains.NotificationHistory;

/// <summary>
/// Mapper class for mapping between NotificationHistoryModel and NotificationResponse.
/// </summary>
public static class NotificationHistoryMapper
{
    /// <summary>
    /// Maps a NotificationHistoryModel to a NotificationResponse.
    /// </summary>
    /// <param name="history">The notification history model.</param>
    /// <returns>The notification response.</returns>
    public static NotificationResponse MapToNotificationHistoryResponse(NotificationHistoryModel history)
    {
        return new NotificationResponse(
            history.Id,
            string.Empty,
            string.Empty,
            [],
            []);
    }

    /// <summary>
    /// Maps a NotificationRequest to a NotificationHistoryModel.
    /// </summary>
    /// <param name="request">The notification request.</param>
    /// <returns>The notification history model.</returns>
    public static NotificationHistoryModel MapToNotificationHistoryModel(NotificationRequest request)
    {
        return new NotificationHistoryModel
        {
            Id = Guid.NewGuid(),
            NotificationId = Guid.Empty,
            IsRead = false
        };
    }
}
