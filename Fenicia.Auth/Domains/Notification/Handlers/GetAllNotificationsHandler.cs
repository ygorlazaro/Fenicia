using Fenicia.Auth.Domains.Notification.Queries;
using Fenicia.Auth.Domains.Notification.Responses;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification.Handlers;

public class GetAllNotificationsHandler(DefaultContext db)
{
    public async Task<Pagination<List<GetAllNotificationsResponse>>> Handle(GetAllNotificationsQuery query, CancellationToken ct)
    {
        var total = await db.AuthNotifications.CountAsync(ct);

        var notifications = await db.AuthNotifications
            .OrderByDescending(n => n.Date)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = notifications.Select(n => new GetAllNotificationsResponse(
            n.Id,
            n.Title,
            n.Description,
            n.Date,
            n.ImageUrl,
            n.Read
        )).ToList();

        return new Pagination<List<GetAllNotificationsResponse>>(response, total, query.Page, query.PerPage);
    }
}
