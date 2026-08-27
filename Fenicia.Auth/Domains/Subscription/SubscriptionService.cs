using Fenicia.Auth.Domains.Subscription.DTOs.Responses;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionService(DefaultContext db)
{
    public async Task<GetUserProfileResponse?> GetUserProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
        {
            return null;
        }

        var userCompanies = await GetUserCompaniesAsync(userId, ct);
        var subscriptions = await GetUserSubscriptionsAsync(userId, ct);

        return new GetUserProfileResponse(user.Id, user.Name, user.Email, userCompanies, subscriptions);
    }

    private async Task<List<UserSubscriptionResponse>> GetUserSubscriptionsAsync(Guid userId, CancellationToken ct)
    {
        var request = from s in db.AuthSubscriptions
                      join c in db.AuthCompanies on s.CompanyId equals c.Id
                      join ur in db.AuthUserRoles on c.Id equals ur.CompanyId
                      where ur.UserId == userId
                      select new UserSubscriptionResponse(s.Id,
                          c.Id,
                          c.Name,
                          s.Status,
                          s.StartDate,
                          s.EndDate);

        var subscriptions = await request.ToListAsync(ct);

        foreach (var subscription in subscriptions)
        {
            var modules = await db.AuthModules
                .Where(m => m.SubscriptionCredits.Any(sc => sc.SubscriptionId == subscription.Id))
                .Select(m => new UserModuleResponse(m.Id, m.Name, m.Type))
                .ToListAsync(ct);

            subscription.Modules = modules;
        }

        return subscriptions;
    }

    private async Task<List<UserCompanyResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken ct)
    {
        var request = from ur in db.AuthUserRoles
                      join c in db.AuthCompanies on ur.CompanyId equals c.Id
                      where ur.UserId == userId
                      select new UserCompanyResponse(c.Id,
                          c.Name,
                          c.Cnpj);

        var companies = await request.ToListAsync(ct);

        return companies;
    }
}
