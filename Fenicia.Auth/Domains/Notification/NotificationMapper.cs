using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Notification;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Notification;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class NotificationMapper
{
    public partial NotificationResponse MapNotificationResponse(NotificationModel notification);
}
