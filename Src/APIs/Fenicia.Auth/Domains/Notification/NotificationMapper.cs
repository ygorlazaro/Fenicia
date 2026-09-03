using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Notification;

public static class NotificationMapper
{
    public static GetAllNotificationsResponse MapToGetAllNotificationsResponse(this NotificationModel notification)
    {
        return new GetAllNotificationsResponse(
            notification.Id,
            notification.Title,
            notification.Description,
            notification.Date,
            notification.ImageUrl,
            notification.Read);
    }

    public static GetNotificationByIdResponse MapToGetNotificationByIdResponse(this NotificationModel notification)
    {
        return new GetNotificationByIdResponse(
            notification.Id,
            notification.Title,
            notification.Description,
            notification.Date,
            notification.ImageUrl,
            notification.Read);
    }
}