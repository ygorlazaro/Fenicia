using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.NotificationHistory;

namespace Fenicia.Auth.Domains.NotificationHistory;

/// <summary>
/// Mapper class for mapping between NotificationHistoryModel and NotificationHistoryResponse/NotificationHistoryRequest.
/// </summary>
public static class NotificationHistoryMapper
{
    /// <summary>
    /// Maps a NotificationHistoryModel to a NotificationHistoryResponse.
    /// </summary>
    /// <param name="history">The notification history model.</param>
    /// <returns>The notification history response.</returns>
    public static NotificationHistoryResponse MapToNotificationHistoryResponse(NotificationHistoryModel history)
    {
        return new NotificationHistoryResponse(
            history.Id,
            history.UserId,
            history.CompanyId,
            history.NotificationId,
            history.ReadAt,
            history.CreatedAt);
    }

    /// <summary>
    /// Maps a NotificationHistoryRequest to a NotificationHistoryModel.
    /// </summary>
    /// <param name="request">The notification history request.</param>
    /// <returns>The notification history model.</returns>
    public static NotificationHistoryModel MapToNotificationHistoryModel(NotificationHistoryRequest request)
    {
        return new NotificationHistoryModel
        {
            Id = request.UserId,
            UserId = request.UserId,
            CompanyId = request.CompanyId,
            NotificationId = request.NotificationId,
            IsRead = request.IsRead
        };
    }
}
