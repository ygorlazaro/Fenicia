using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification;

public class NotificationRepository(DefaultContext context)
    : Repository<NotificationModel>(context), INotificationRepository
{
    public async Task<Pagination<List<NotificationModel>>> GetAllWithPaginationAsync(
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        var query = from n in DbSet
            orderby n.Date descending
            select n;

        var totalTask = query.CountAsync(cancellationToken);
        var itemsTask = query.Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);

        await Task.WhenAll(totalTask, itemsTask);

        return new Pagination<List<NotificationModel>>(itemsTask.Result, totalTask.Result, page, perPage);
    }
}