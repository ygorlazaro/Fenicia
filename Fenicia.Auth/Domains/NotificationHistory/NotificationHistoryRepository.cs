using Fenicia.Auth.Domains.NotificationHistory.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.NotificationHistory;

#pragma warning disable IDE0290
public class NotificationHistoryRepository : Repository<NotificationHistoryModel>, INotificationHistoryRepository
{
    public NotificationHistoryRepository(DbContext context) : base(context)
    {
    }
#pragma warning restore IDE0290

    public async Task<bool> MaskReadStatusAsync(Guid notificationId, Guid userId, Guid companyId, bool readStatus,
        CancellationToken cancellationToken)
    {
        var query = from nh in DbSet
            where nh.NotificationId == notificationId
                && nh.UserId == userId
                && nh.CompanyId == companyId
            select nh;

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return false;
        }

        result.IsRead = readStatus;
        Context.Entry(result).State = EntityState.Modified;

        await SaveChangesAsync(cancellationToken);

        return true;
    }

    public Task<bool> IsReadAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        return DbSet.AnyAsync(nh => nh.Id == notificationId && nh.IsRead, cancellationToken);
    }
}
