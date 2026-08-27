using Fenicia.Auth.Domains.Notification.DTOs.Responses;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Notification.DTOs.Queries;

public record GetAllNotificationsQuery(int Page = 1, int PerPage = 10);
