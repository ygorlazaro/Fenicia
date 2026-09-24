using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription;

/// <summary>
/// Repository implementation for managing subscriptions in the authentication domain.
/// </summary>
/// <param name="context">The database context.</param>
public class SubscriptionRepository(DbContext context)
    : Repository<SubscriptionModel>(context), ISubscriptionRepository
{
    /// <summary>
    /// Gets all subscriptions for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of subscriptions.</returns>
    public Task<List<SubscriptionModel>> GetUserSubscriptionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return DbSet
            .Include(s => s.Company)
            .Where(s => s.Company.UsersRoles.Any(ur => ur.UserId == userId))
            .Where(s => s.Status == EnumSubscriptionStatus.Active && now >= s.StartDate && now <= s.EndDate)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets all active subscriptions for a specific company.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of subscriptions.</returns>
    public Task<List<SubscriptionModel>> GetActiveSubscriptionsByCompanyAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var query = from s in DbSet
                    where s.CompanyId == companyId
                          && s.Status == EnumSubscriptionStatus.Active
                          && now >= s.StartDate
                          && now <= s.EndDate
                    select s;

        return query.ToListAsync(cancellationToken);
    }
}
