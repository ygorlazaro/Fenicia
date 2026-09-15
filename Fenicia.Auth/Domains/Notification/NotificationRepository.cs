using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification;

public class NotificationRepository(DbContext context)
    : Repository<NotificationModel>(context), INotificationRepository
{
    public new async Task<IEnumerable<NotificationModel>> GetAllAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        var query = from n in DbSet
            orderby n.Date descending
            select n;

        return await query
            .Skip((perPage - 1)  * page)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }
}