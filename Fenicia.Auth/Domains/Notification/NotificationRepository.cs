using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification;

public class NotificationRepository(DefaultContext context) : Repository<NotificationModel>(context)
{
    public async Task<Pagination<List<NotificationModel>>> GetAllWithPaginationAsync(int page, int perPage, CancellationToken ct = default)
    {
        var query = from n in DbSet
                    orderby n.Date descending
                    select n;

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * perPage).Take(perPage).ToListAsync(ct);

        return new Pagination<List<NotificationModel>>(items, total, page, perPage);
    }
}
