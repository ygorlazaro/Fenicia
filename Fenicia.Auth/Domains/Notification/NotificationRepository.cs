using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification;

public class NotificationRepository(DefaultContext context)
    : Repository<NotificationModel>(context), INotificationRepository
{
    public async Task<IEnumerable<NotificationModel>> GetAllAsync(Guid companyId, Guid userId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        var query = from n in DbSet
            join nh in context.AuthNotificationHistory on n.Id equals nh.NotificationId
            where nh.CompanyId == companyId || nh.UserId == userId
            orderby n.Date descending
            select n;

        return await query
            .Skip((perPage - 1)  * page)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public Task<NotificationModel?> GetByIdAsync(Guid id, Guid companyId, Guid userId, CancellationToken cancellationToken)
    {
        var query = from n in DbSet
            join nh in context.AuthNotificationHistory on n.Id equals nh.NotificationId
            where (nh.CompanyId == companyId || nh.UserId == userId)
            && n.Id == id
            orderby n.Date descending
            select n;

        return query.FirstOrDefaultAsync(cancellationToken);
    }
}